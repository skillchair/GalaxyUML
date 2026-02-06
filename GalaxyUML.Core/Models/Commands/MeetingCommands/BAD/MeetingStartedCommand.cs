// using GalaxyUML.Core.Models.Commands.MeetingCommands;

// namespace GalaxyUML.Core.Models.Commands.TeamCommands
// {
//     class MeetingStartedCommand : IMeetingCommand
//     {
//         public TeamMember Organizer { get; private set; }

//         public MeetingStartedCommand(Meeting meeting, TeamMember organizer) : base(meeting)
//         {
//             Organizer = organizer;
//         }

//         public override void Execute(MeetingEventType eventType)
//         {
//             if (eventType != MeetingEventType.MeetingStarted)
//                 throw new Exception("Invalid event. Expected MeetingEventType.MeetingStarted.");

//             Meeting.AddParticipant(Organizer);
//             Meeting.GrantControl(Organizer);
//         }
//     }
// }