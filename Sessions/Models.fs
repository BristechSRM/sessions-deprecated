namespace Sessions.Models

type Rating = 
    | Zero = 0
    | One = 1
    | Two = 2
    | Three = 3
    | Four = 4
    | Five = 5

type TalkStatus = 
    | Unassigned = 0
    | Assigned = 1
    | InProgress = 2
    | Deferred = 3
    | TopicApproved = 4
    | DateAssigned = 5

type TalkOutline = {
    TalkId : int
    Title : string
    Status: TalkStatus
    SpeakerName : string
    SpeakerEmail : string
    SpeakerRating: Rating
    AdminName: string
    AdminImageUrl: string
}

