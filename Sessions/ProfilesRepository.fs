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

let getProfile (profileId : Guid) = 
    try 
        use connection = getConnection()
        connection.Open()
        
        let profileEntity = connection.Get<ProfileEntity>(profileId)
        let handleEntities = connection.Query<HandleEntity>("select type,identifier,profileId from handles where profileId = @Id", dict ["Id", box profileId ] )

        connection.Close()

        if box profileEntity |> isNull then
            Failure { HttpStatus = HttpStatusCode.NotFound
                      Message = sprintf "Profile with id %A does not exist" profileId }
        else 
            let profile = entityToModel profileEntity handleEntities
            Success profile
    with
    | ex ->
        Log.Error("getProfile(profileId) - Exception: {0}", ex)
        Failure { HttpStatus = HttpStatusCode.InternalServerError
                  Message = ex.Message }

let addProfile (profile : Profile) = 
    try 
        let profileEntity = modelToEntity { profile with Id = Guid.NewGuid() }
        let handleEntities = profile.Handles |> Seq.map (handleModelToEntity profileEntity.Id)

        use connection = getConnection()
        connection.Open()    
        use transaction = connection.BeginTransaction()

        let profileInsertCount = connection.Execute(@"insert profiles(id,forename,surname,rating,imageUrl) values (@Id,@Forename,@Surname,@Rating,@ImageUrl)", profileEntity)
        if profileInsertCount <> 1 then failwith "Profile insert failed"

        let handlesInsertCount = connection.Execute(@"insert handles(type,identifier,profileId) values (@Type,@Identifier,@ProfileId)", handleEntities)
        if handlesInsertCount <> Seq.length profile.Handles then failwith "Incorrect number of inserted handles found.  Handles insert failed"

        transaction.Commit()
        connection.Close()
        Success profileEntity.Id
    with
    | ex ->
        Log.Error("addProfile() - Exception: {0}", ex)
        Failure { HttpStatus = HttpStatusCode.BadRequest
                  Message = ex.Message }

let updateProfile (pid: Guid) (profile : Profile) = 
    if pid <> profile.Id then 
        Failure { HttpStatus = HttpStatusCode.BadRequest
                  Message = "Invalid Data. specified profile Id in request url does not match Id of input profile" } 
    else 
        let handleDoesNotExistIn handles handle = 
            handles |> Seq.exists (fun hl -> hl.Type = handle.Type && hl.Identifier = handle.Identifier) |> not
        try 
            match getProfile pid with
            | Success _ ->
                let profileEntity = modelToEntity profile
                let handleEntities = profile.Handles |> Seq.map (handleModelToEntity profileEntity.Id)

                use connection = getConnection()
                connection.Open()
                use transaction = connection.BeginTransaction()

                let profileUpdateCount = connection.Execute(@"update profiles set forename=@Forename,surname=@Surname,rating=@Rating,imageurl=@ImageUrl,bio=@Bio where Id = @Id",profileEntity)
                if profileUpdateCount <> 1 then failwith "Update of profile data failed"

                let storedHandles = connection.Query<HandleEntity>("select type,identifier,profileId from handles where profileId = @Id", dict [ "Id", box profile.Id ]) |> Seq.toList

                let newHandles = handleEntities |> Seq.filter (handleDoesNotExistIn storedHandles)

                let deletedHandles = storedHandles |> Seq.filter (handleDoesNotExistIn handleEntities)

                let handlesInsertCount = connection.Execute(@"insert handles(type,identifier,profileId) values (@Type,@Identifier,@ProfileId)", newHandles)
                if handlesInsertCount <> Seq.length newHandles then failwith "Incorrect number of inserted handles found. Handles insert failed"

                let handlesDeleteCount = connection.Execute(@"delete from handles where profileId=@ProfileId and identifier=@Identifier and type=@type",deletedHandles)
                if handlesDeleteCount <> Seq.length deletedHandles then failwith "Incorrect number of deleted handles found. Handles delete failed"

                transaction.Commit()
                connection.Close()
                Success ()

            | Failure error -> 
                match error.HttpStatus with
                | HttpStatusCode.NotFound -> 
                    Failure { HttpStatus = HttpStatusCode.NotFound
                              Message = sprintf "No update performed. Profile with id: %A does not exist. Put is update only" pid}        
                | _ -> Failure error
        with
        | ex -> 
            Log.Error("updateProfile() - Exception {0}", ex)
            Failure { HttpStatus = HttpStatusCode.InternalServerError
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
        Failure { HttpStatus = HttpStatusCode.InternalServerError
                  Message = ex.Message }


let getHandle (handletype : string) (identifier : string) = 
    try
        use connection = getConnection()
        connection.Open()
        
        let handles = connection.Query<HandleEntity>("select profileId, type, identifier from handles where type = @Type and identifier = @Identifier", dict["Type", box handletype; "Identifier", box identifier])
        if handles |> Seq.isEmpty |> not then 
            handles |> Seq.head |> Success
        else Failure {
            HttpStatus = HttpStatusCode.NotFound
            Message = sprintf "A handle with type %s and identifier %s could not be found" handletype identifier }
    with
    | ex ->
        Log.Error("getHandle(handletype) - Exception: {0}", ex)
        Failure { HttpStatus = HttpStatusCode.InternalServerError
                  Message = ex.Message }
