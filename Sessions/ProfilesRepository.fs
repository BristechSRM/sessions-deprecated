module ProfilesRepository

open System
open Dapper
open Dapper.Contrib.Extensions
open System.Configuration
open MySql.Data.MySqlClient
open Sessions.Models
open Sessions.Entities

let connectionString = ConfigurationManager.ConnectionStrings.Item("DefaultConnection").ConnectionString

let entityToModel (entity : ProfileEntity) : Profile = 
    { Id = entity.Id
      Forename = entity.Forename
      Surname = entity.Surname
      Rating = enum entity.Rating
      Handles = [||] }

let modelToEntity (model : Profile) : ProfileEntity = 
    {   ProfileEntity.Id = model.Id
        Forename = model.Forename
        Surname = model.Surname
        Rating = (int) model.Rating }

//TODO result DU
//TODO handles
//TODO returning error messages and exception handling fully

let getProfile (id : Guid) = 
    use connection = new MySqlConnection(connectionString)
    connection.Open()
    try 
        let result = connection.Get<ProfileEntity>(id) |> entityToModel
        connection.Close()
        Some result
    with :? System.Exception as ex -> 
        connection.Close()
        None

let addProfile (profile : Profile) = 
    let id = Guid.NewGuid()    
    let profile = modelToEntity {profile with Id = id }
    
    use connection = new MySqlConnection(connectionString)
    connection.Open()
    use transaction = connection.BeginTransaction()
    try 
        let result = connection.Execute(@"insert profiles(id,forename,surname,rating) values (@Id,@Forename,@Surname,@Rating)", profile)
        if result <> 1 then failwith "Unknown failure. Profile insert failed"
        transaction.Commit()
        connection.Close()
        Some id
    with :? System.Exception as ex -> 
        transaction.Rollback()
        connection.Close()
        None
