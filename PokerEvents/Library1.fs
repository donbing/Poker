namespace PokerEvents

type Class1() = 
    member this.X = "F#"

type PlayerAdded(firstName:string) = 
    member this.FirstName = firstName

type PlayerReady(firstName, middleInitial, lastName) = 
    member this.FirstName = firstName
    member this.MiddleInitial = middleInitial
    member this.LastName = lastName  

type SeatRequested(firstName, middleInitial, lastName) = 
    member this.FirstName = firstName
    member this.MiddleInitial = middleInitial
    member this.LastName = lastName  
