namespace Speakers.Controllers

open System.Web.Http
open Speakers.Models 

type SpeakersController() =
    inherit ApiController()

    member x.Get() =
        printfn "Received Get Request for Speakers"
        seq {
            yield {Name = "Thomas Hull"; Title = "To know javascript is to love javascript"; Rating=5; Admin="David Wybourn"; AdminImageUrl="https://placebear.com/50/50"; LastContacted="1970-01-01"; SpeakerStatus = SpeakerStatus.Assigned }
            yield {Name = "Jason Ebbin";   Title = "How dull is F#"; Rating=3; Admin="Jason Ebbin"; AdminImageUrl="https://placebear.com/50/50"; LastContacted="1989-11-09"; SpeakerStatus = SpeakerStatus.Deferred }
            yield {Name = "David Wybourn"; Title = "Concourse: Where I met myself"; Rating=5; Admin="Chris James Smith"; AdminImageUrl="https://placebear.com/50/50"; LastContacted="2015-10-21"; SpeakerStatus = SpeakerStatus.TopicApproved }
            yield {Name = "Joe Bloggs"; Title = ""; Rating=0; Admin="Thomas Hull"; AdminImageUrl="https://placebear.com/50/50"; LastContacted="2016-02-19"; SpeakerStatus = SpeakerStatus.InProgress }
            yield {Name = "Chris Smith"; Title = "C# or F#: Which is sharper?"; Rating=4; Admin="Thomas Hull"; AdminImageUrl="https://placebear.com/50/50"; LastContacted="2016-01-10"; SpeakerStatus = SpeakerStatus.DateAssigned }
        }
