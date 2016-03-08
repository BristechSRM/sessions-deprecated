namespace Speakers.Controllers

open System
open System.Web.Http
open Speakers.Models 

type SpeakersController() =
    inherit ApiController()

    member x.Get() =
        printfn "Received Get Request for Speakers"
        seq {
            yield {Name = "Thomas Hull"; Title = "To know javascript is to love javascript"; Rating=Rating.Five; Admin="David Wybourn"; AdminImageUrl="https://placebear.com/50/50"; LastContacted=DateTime(1970,1,1); SpeakerStatus = SpeakerStatus.Assigned }
            yield {Name = "Jason Ebbin";   Title = "F#: The sharpest tool in the shed"; Rating=Rating.Five; Admin="Jason Ebbin"; AdminImageUrl="https://placebear.com/50/50"; LastContacted=DateTime(1989,11,09); SpeakerStatus = SpeakerStatus.Deferred }
            yield {Name = "David Wybourn"; Title = "Concourse: Where I met myself"; Rating=Rating.Three; Admin="Chris James Smith"; AdminImageUrl="https://placebear.com/50/50"; LastContacted=DateTime(2015,10,21); SpeakerStatus = SpeakerStatus.TopicApproved }
            yield {Name = "Joe Bloggs"; Title = ""; Rating=Rating.Zero; Admin="Thomas Hull"; AdminImageUrl="https://placebear.com/50/50"; LastContacted=DateTime(2016,02,19); SpeakerStatus = SpeakerStatus.InProgress }
            yield {Name = "Chris Smith"; Title = "C# or F#: Which is sharper?"; Rating=Rating.One; Admin="Thomas Hull"; AdminImageUrl="https://placebear.com/50/50"; LastContacted=DateTime(2016,01,10); SpeakerStatus = SpeakerStatus.DateAssigned }
        }
