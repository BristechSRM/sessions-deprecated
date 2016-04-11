module ProfilesRepository

open System
open Dapper
open Dapper.Contrib.Extensions
open System.Configuration
open MySql.Data.MySqlClient
open Sessions.Models
open Sessions.Entities

let connectionString = ConfigurationManager.ConnectionStrings.Item("DefaultConnection").ConnectionString

let entityToProfile (entity : ProfileEntity) : Profile = 
    { Id = entity.Id
      Forename = entity.Forename
      Surname = entity.Surname
      Rating = enum entity.Rating
      Handles = [||] }

//TODO result DU
//TODO handles
//TODO returning error messages and exception handling fully

let getProfile (id : Guid) = 
    use connection = new MySqlConnection(connectionString)
    connection.Open()
    try 
        let result = connection.Get<ProfileEntity>(id)
        connection.Close()
        Some result
    with :? System.Exception as ex -> 
        connection.Close()
        None

let addProfile (profile : Profile) = 
    let id = Guid.NewGuid()
    
    let profile = 
        { ProfileEntity.Id = id
          Forename = profile.Forename
          Surname = profile.Surname
          Rating = (int) profile.Rating }
    
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
