module ProfilesRepository

open System
open System.Net
open Dapper
open Dapper.Contrib.Extensions
open System.Configuration
open System.Data.SqlClient
open MySql.Data.MySqlClient
open Models
open Entities
open Serilog

let connectionString = ConfigurationManager.ConnectionStrings.Item("DefaultConnection").ConnectionString
let getConnection() = new MySqlConnection(connectionString)

let handleEntityToModel (entity : HandleEntity) = 
    { Handle.Type = entity.Type
      Identifier = entity.Identifier }

let handleModelToEntity profileId (model : Handle) = 
    { HandleEntity.Identifier = model.Identifier
      Type = model.Type
      ProfileId = profileId }

let entityToModel (entity : ProfileEntity) handles : Profile = 
    { Id = entity.Id
      Forename = entity.Forename
      Surname = entity.Surname
      Rating = enum entity.Rating
      ImageUrl = entity.ImageUrl
      Bio = entity.Bio
      Handles = handles |> Seq.map handleEntityToModel }

let modelToEntity (model : Profile) : ProfileEntity = 
    { ProfileEntity.Id = model.Id
      Forename = model.Forename
      Surname = model.Surname
      Rating = (int) model.Rating
      ImageUrl = model.ImageUrl
      Bio = model.Bio }


type IdWrapper = 
    { Id : Guid }

let getProfile (profileId : Guid) = 
    try 
        use connection = getConnection()
        connection.Open()
        
        let handles = connection.Query<HandleEntity>("select type,identifier,profileId from handles where profileId = @Id", { Id = profileId })
        let profileEntity = connection.Get<ProfileEntity>(profileId)
        let profile = entityToModel profileEntity handles
        connection.Close()
        Success profile
    with
    | ex ->
        Log.Error("getProfile(profileId) - Exception: {0}", ex)
        Failure { HttpStatus = HttpStatusCode.BadRequest
                  Message = ex.Message }


let addProfile (profile : Profile) = 
    try 
        let newId = Guid.NewGuid()
        let profileEntity = modelToEntity { profile with Id = newId }
        use connection = getConnection()
        connection.Open()
    
        use transaction = connection.BeginTransaction()
        let profileInsertCount = connection.Execute(@"insert profiles(id,forename,surname,rating,imageUrl) values (@Id,@Forename,@Surname,@Rating,@ImageUrl)", profileEntity)
        if profileInsertCount <> 1 then failwith "Incorrect number of inserted profiles found. Profile insert failed"
        let handleEntities = profile.Handles |> Seq.map (handleModelToEntity newId)
        let handlesInsertCount = connection.Execute(@"insert handles(type,identifier,profileId) values (@Type,@Identifier,@ProfileId)", handleEntities)
        if handlesInsertCount <> Seq.length profile.Handles then failwith "Incorrect number of inserted handles found.  Handles insert failed"
        transaction.Commit()
        connection.Close()
        Success newId
    with
    | ex ->
        Log.Error("addProfile() - Exception: {0}", ex)
        Failure { HttpStatus = HttpStatusCode.BadRequest
                  Message = ex.Message }


let getHandles() = 
    try
        use connection = getConnection()
        connection.Open()
        
        let handles = connection.Query<HandleEntity>("select profileId, type, identifier from handles order by type, identifier")
        handles 
        |> Success
    with
    | ex ->
        Log.Error("getHandles() - Exception: {0}", ex)
        Failure { HttpStatus = HttpStatusCode.BadRequest
                  Message = ex.Message }


let getHandle (handletype : string) (identifier : string) = 
    try
        use connection = getConnection()
        connection.Open()
        
        let cmd = String.Format("select profileId, type, identifier from handles where type = '{0}' and identifier = '{1}'", handletype, identifier)
        let handles = connection.Query<HandleEntity>(cmd)
        if not ( Seq.isEmpty handles ) then 
            handles |> Seq.head |> Success
        else Failure {
            HttpStatus = HttpStatusCode.NotFound
            Message = "" }
    with
    | ex ->
        Log.Error("getHandle(handletype) - Exception: {0}", ex)
        Failure { HttpStatus = HttpStatusCode.BadRequest
                  Message = ex.Message }
