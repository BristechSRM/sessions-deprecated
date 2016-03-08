namespace Speakers.Models

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
    Rating: int
    Admin: string
    AdminImageUrl: string
    LastContacted: string
    SpeakerStatus: SpeakerStatus
}

