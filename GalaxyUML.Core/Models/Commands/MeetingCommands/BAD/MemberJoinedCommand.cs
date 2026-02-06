// namespace GalaxyUML.Core.Models.Commands.MeetingCommands
// {
//     public class MemberJoinedCommand: IMeetingCommand
//     {
//         public MeetingParticipant NewParticipant { get; private set; }
//         public List<MeetingParticipant> Participants { get; private set; }

//         public MemberJoinedCommand(Meeting meeting, MeetingParticipant newParticipant, List<MeetingParticipant> participants) : base(meeting)
//         {
//             NewParticipant = newParticipant;
//             Participants = participants;
//         }

//         public override void Execute(MeetingEventType eventType)
//         {
//             if (eventType != MeetingEventType.MeetingStarted)
//                 throw new Exception("Invalid event. Expected MeetingEventType.MeetingStarted.");

//             Participants.Add(NewParticipant);
//         }
//     }
// }