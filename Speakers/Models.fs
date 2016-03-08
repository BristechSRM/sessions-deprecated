namespace Speakers.Models

open System

type Rating =
    | Zero = 0
    | One = 1
    | Two = 2
    | Three = 3
    | Four = 4
    | Five = 5

type SpeakerStatus =
    | Unassigned = 0
    | Assigned = 1
    | InProgress = 2
    | Deferred = 3
    | TopicApproved = 4
    | DateAssigned = 5

type Speaker = {
    Name : string
    Title : string
    Rating: Rating
    Admin: string
    AdminImageUrl: string
    LastContacted: DateTime
    SpeakerStatus: SpeakerStatus
}

