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

let entityToModel (entity : ProfileEntity) : Profile = 
    { Id = entity.Id
      Forename = entity.Forename
      Surname = entity.Surname
      Rating = enum entity.Rating
      ImageUrl = entity.ImageUrl
      Handles = [||] }

let modelToEntity (model : Profile) : ProfileEntity = 
    {   ProfileEntity.Id = model.Id
        Forename = model.Forename
        Surname = model.Surname
        Rating = (int) model.Rating 
        ImageUrl = model.ImageUrl}

//TODO handles

let getProfile (id : Guid) = 
    use connection = getConnection() 
    connection.Open()
    try 
        let result = connection.Get<ProfileEntity>(id) |> entityToModel
        connection.Close()
        Success result
    with 
        | :? SqlException as ex -> 
            connection.Close()
            Failure { HttpStatus = HttpStatusCode.BadRequest; Message = ex.Message}
        | _-> 
            connection.Close()
            Failure { HttpStatus = HttpStatusCode.InternalServerError; Message = ""}

let addProfile (profile : Profile) = 
    let id = Guid.NewGuid()    
    let profile = modelToEntity {profile with Id = id }
    
    use connection = getConnection() 
    connection.Open()
    use transaction = connection.BeginTransaction()
    try 
        let result = connection.Execute(@"insert profiles(id,forename,surname,rating) values (@Id,@Forename,@Surname,@Rating)", profile)
        if result <> 1 then failwith "Unknown failure. Profile insert failed"
        transaction.Commit()
        connection.Close()
        Success id
    with 
        | :? SqlException as ex -> 
            transaction.Rollback()
            connection.Close()
            Failure { HttpStatus = HttpStatusCode.BadRequest; Message = ex.Message}
        | _-> 
            transaction.Rollback()
            connection.Close()
            Failure { HttpStatus = HttpStatusCode.InternalServerError; Message = ""}
