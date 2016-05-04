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
    use connection = getConnection()
    connection.Open()
    try 
        let handles = connection.Query<HandleEntity>("select type,identifier,profileId from handles where profileId = @Id", { Id = profileId })
        let profileEntity = connection.Get<ProfileEntity>(profileId)
        let profile = entityToModel profileEntity handles
        connection.Close()
        Success profile
    with
    | :? SqlException as ex -> 
        connection.Close()
        Failure { HttpStatus = HttpStatusCode.BadRequest
                  Message = ex.Message }
    | ex -> 
        connection.Close()
        Failure { HttpStatus = HttpStatusCode.InternalServerError
                  Message = "Unhandled error: " + ex.Message }

let addProfile (profile : Profile) = 
    let newId = Guid.NewGuid()
    let profileEntity = modelToEntity { profile with Id = newId }
    use connection = getConnection()
    connection.Open()
    use transaction = connection.BeginTransaction()
    try 
        let profileInsertCount = connection.Execute(@"insert profiles(id,forename,surname,rating,imageUrl) values (@Id,@Forename,@Surname,@Rating,@ImageUrl)", profileEntity)
        if profileInsertCount <> 1 then failwith "Incorrect number of inserted profiles found. Profile insert failed"
        let handleEntities = profile.Handles |> Seq.map (handleModelToEntity newId)
        let handlesInsertCount = connection.Execute(@"insert handles(type,identifier,profileId) values (@Type,@Identifier,@ProfileId)", handleEntities)
        if handlesInsertCount <> Seq.length profile.Handles then failwith "Incorrect number of inserted handles found.  Handles insert failed"
        transaction.Commit()
        connection.Close()
        Success newId
    with
    | :? SqlException as ex -> 
        transaction.Rollback()
        connection.Close()
        Failure { HttpStatus = HttpStatusCode.BadRequest
                  Message = ex.Message }
    | _ -> 
        transaction.Rollback()
        connection.Close()
        Failure { HttpStatus = HttpStatusCode.InternalServerError
                  Message = "Internal Server Error" }
