namespace TipsAndSteps.Shared.Contracts;

public static class KafkaTopics
{
    // User & Profile events
    public const string UserRegistered       = "tns.user.registered";
    public const string UserProfileUpdated   = "tns.user.profile-updated";
    public const string ChildProfileCreated  = "tns.child.profile-created";
    public const string ChildProfileUpdated  = "tns.child.profile-updated";

    // Content events
    public const string ContentViewed        = "tns.content.viewed";
    public const string ContentPublished     = "tns.content.published";
    public const string ContentRated         = "tns.content.rated";

    // Growth Matrix events
    public const string GrowthAssessmentCompleted = "tns.growth.assessment-completed";
    public const string GrowthAlertTriggered      = "tns.growth.alert-triggered";

    // Q&A events
    public const string QuestionSubmitted    = "tns.qa.question-submitted";
    public const string QuestionAnswered     = "tns.qa.question-answered";

    // Notification events
    public const string NotificationRequested = "tns.notification.requested";
    public const string NotificationSent      = "tns.notification.sent";

    // Health Intelligence
    public const string HealthDataReported   = "tns.health.data-reported";

    // Analytics Projections (internal)
    public const string AnalyticsDailyMetrics    = "tns.analytics.daily-metrics";
    public const string AnalyticsContentEngagement = "tns.analytics.content-engagement";
}
