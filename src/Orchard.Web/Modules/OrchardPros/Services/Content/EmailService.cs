using System.Collections.Generic;
using Orchard.DisplayManagement;
using Orchard.Email.Services;
using Orchard.JobsQueue.Services;
using Orchard.Localization;

namespace OrchardPros.Services.Content {
    public class EmailService : IEmailService {
        private readonly IShapeFactory _shapeFactory;
        private readonly IShapeDisplay _shapeDisplay;
        private readonly IJobsQueueService _jobsQueueService;

        public EmailService(IShapeFactory shapeFactory, IShapeDisplay shapeDisplay, IJobsQueueService jobsQueueService) {
            _shapeFactory = shapeFactory;
            _shapeDisplay = shapeDisplay;
            _jobsQueueService = jobsQueueService;
        }

        public void Queue(LocalizedString subject, string recipientEmail, string templateName, object templateProperties) {
            var template = _shapeFactory.Create(templateName, Arguments.From(templateProperties));

            var messageParameters = new Dictionary<string, object> {
                {"Subject", subject.Text},
                {"Body", _shapeDisplay.Display(template)},
                {"Recipients", recipientEmail }
            };
            var jobParameters = new {
                type = SmtpMessageChannel.MessageType,
                parameters = messageParameters
            };

            _jobsQueueService.Enqueue("IMessageService.Send", jobParameters, 0);
        }
    }
}