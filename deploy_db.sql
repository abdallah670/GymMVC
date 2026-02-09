IF OBJECT_ID(N'[__EFMigrationsHistory]') IS NULL
BEGIN
    CREATE TABLE [__EFMigrationsHistory] (
        [MigrationId] nvarchar(150) NOT NULL,
        [ProductVersion] nvarchar(32) NOT NULL,
        CONSTRAINT [PK___EFMigrationsHistory] PRIMARY KEY ([MigrationId])
    );
END;
GO

BEGIN TRANSACTION;
INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20251125152501_Adding Payment, Membership and some classes', N'9.0.10');

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20251125160052_Editing Memebership', N'9.0.10');

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20251125161119_Fix Membership', N'9.0.10');

CREATE TABLE [AspNetRoles] (
    [Id] nvarchar(450) NOT NULL,
    [Name] nvarchar(256) NULL,
    [NormalizedName] nvarchar(256) NULL,
    [ConcurrencyStamp] nvarchar(max) NULL,
    CONSTRAINT [PK_AspNetRoles] PRIMARY KEY ([Id])
);

CREATE TABLE [AspNetUsers] (
    [Id] nvarchar(450) NOT NULL,
    [FullName] nvarchar(100) NOT NULL,
    [Phone] nvarchar(20) NULL,
    [ProfilePicture] nvarchar(max) NULL,
    [IsActive] bit NOT NULL,
    [UserName] nvarchar(256) NULL,
    [NormalizedUserName] nvarchar(256) NULL,
    [Email] nvarchar(256) NULL,
    [NormalizedEmail] nvarchar(256) NULL,
    [EmailConfirmed] bit NOT NULL,
    [PasswordHash] nvarchar(max) NULL,
    [SecurityStamp] nvarchar(max) NULL,
    [ConcurrencyStamp] nvarchar(max) NULL,
    [PhoneNumber] nvarchar(max) NULL,
    [PhoneNumberConfirmed] bit NOT NULL,
    [TwoFactorEnabled] bit NOT NULL,
    [LockoutEnd] datetimeoffset NULL,
    [LockoutEnabled] bit NOT NULL,
    [AccessFailedCount] int NOT NULL,
    CONSTRAINT [PK_AspNetUsers] PRIMARY KEY ([Id])
);

CREATE TABLE [DietPlans] (
    [Id] int NOT NULL IDENTITY,
    [Name] nvarchar(100) NOT NULL,
    [Description] nvarchar(1000) NULL,
    [TotalCalories] int NULL DEFAULT 2000,
    [ProteinMacros] nvarchar(100) NULL,
    [CarbMacros] nvarchar(100) NULL,
    [FatMacros] nvarchar(100) NULL,
    [DietType] nvarchar(50) NOT NULL DEFAULT N'Balanced',
    [DurationDays] int NOT NULL,
    [IsActive] bit NOT NULL DEFAULT CAST(1 AS bit),
    CONSTRAINT [PK_DietPlans] PRIMARY KEY ([Id])
);

CREATE TABLE [WorkoutPlans] (
    [Id] int NOT NULL IDENTITY,
    [Name] nvarchar(100) NOT NULL,
    [Description] nvarchar(1000) NULL,
    [Difficulty] nvarchar(50) NOT NULL DEFAULT N'Beginner',
    [Goal] nvarchar(50) NOT NULL DEFAULT N'General Fitness',
    [IsActive] bit NOT NULL DEFAULT CAST(1 AS bit),
    CONSTRAINT [PK_WorkoutPlans] PRIMARY KEY ([Id])
);

CREATE TABLE [AspNetRoleClaims] (
    [Id] int NOT NULL IDENTITY,
    [RoleId] nvarchar(450) NOT NULL,
    [ClaimType] nvarchar(max) NULL,
    [ClaimValue] nvarchar(max) NULL,
    CONSTRAINT [PK_AspNetRoleClaims] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_AspNetRoleClaims_AspNetRoles_RoleId] FOREIGN KEY ([RoleId]) REFERENCES [AspNetRoles] ([Id]) ON DELETE CASCADE
);

CREATE TABLE [AspNetUserClaims] (
    [Id] int NOT NULL IDENTITY,
    [UserId] nvarchar(450) NOT NULL,
    [ClaimType] nvarchar(max) NULL,
    [ClaimValue] nvarchar(max) NULL,
    CONSTRAINT [PK_AspNetUserClaims] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_AspNetUserClaims_AspNetUsers_UserId] FOREIGN KEY ([UserId]) REFERENCES [AspNetUsers] ([Id]) ON DELETE CASCADE
);

CREATE TABLE [AspNetUserLogins] (
    [LoginProvider] nvarchar(450) NOT NULL,
    [ProviderKey] nvarchar(450) NOT NULL,
    [ProviderDisplayName] nvarchar(max) NULL,
    [UserId] nvarchar(450) NOT NULL,
    CONSTRAINT [PK_AspNetUserLogins] PRIMARY KEY ([LoginProvider], [ProviderKey]),
    CONSTRAINT [FK_AspNetUserLogins_AspNetUsers_UserId] FOREIGN KEY ([UserId]) REFERENCES [AspNetUsers] ([Id]) ON DELETE CASCADE
);

CREATE TABLE [AspNetUserRoles] (
    [UserId] nvarchar(450) NOT NULL,
    [RoleId] nvarchar(450) NOT NULL,
    CONSTRAINT [PK_AspNetUserRoles] PRIMARY KEY ([UserId], [RoleId]),
    CONSTRAINT [FK_AspNetUserRoles_AspNetRoles_RoleId] FOREIGN KEY ([RoleId]) REFERENCES [AspNetRoles] ([Id]) ON DELETE CASCADE,
    CONSTRAINT [FK_AspNetUserRoles_AspNetUsers_UserId] FOREIGN KEY ([UserId]) REFERENCES [AspNetUsers] ([Id]) ON DELETE CASCADE
);

CREATE TABLE [AspNetUserTokens] (
    [UserId] nvarchar(450) NOT NULL,
    [LoginProvider] nvarchar(450) NOT NULL,
    [Name] nvarchar(450) NOT NULL,
    [Value] nvarchar(max) NULL,
    CONSTRAINT [PK_AspNetUserTokens] PRIMARY KEY ([UserId], [LoginProvider], [Name]),
    CONSTRAINT [FK_AspNetUserTokens_AspNetUsers_UserId] FOREIGN KEY ([UserId]) REFERENCES [AspNetUsers] ([Id]) ON DELETE CASCADE
);

CREATE TABLE [Members] (
    [Id] nvarchar(450) NOT NULL,
    [JoinDate] date NOT NULL DEFAULT (GETUTCDATE()),
    CONSTRAINT [PK_Members] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_Members_AspNetUsers_Id] FOREIGN KEY ([Id]) REFERENCES [AspNetUsers] ([Id]) ON DELETE CASCADE
);

CREATE TABLE [Notifications] (
    [Id] int NOT NULL IDENTITY,
    [UserId] nvarchar(450) NOT NULL,
    [Type] nvarchar(450) NOT NULL,
    [Message] nvarchar(500) NOT NULL,
    [Status] nvarchar(450) NOT NULL DEFAULT N'Unread',
    [SendTime] datetime2 NOT NULL DEFAULT (GETUTCDATE()),
    [DeliveryMethod] nvarchar(450) NOT NULL DEFAULT N'InApp',
    [RelatedEntity] nvarchar(100) NULL,
    [RelatedEntityId] int NULL,
    [IsActive] bit NOT NULL,
    CONSTRAINT [PK_Notifications] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_Notifications_AspNetUsers_UserId] FOREIGN KEY ([UserId]) REFERENCES [AspNetUsers] ([Id]) ON DELETE CASCADE
);

CREATE TABLE [Trainers] (
    [Id] nvarchar(450) NOT NULL,
    [ExperienceYears] int NULL,
    [Bio] nvarchar(1000) NULL,
    CONSTRAINT [PK_Trainers] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_Trainers_AspNetUsers_Id] FOREIGN KEY ([Id]) REFERENCES [AspNetUsers] ([Id]) ON DELETE CASCADE
);

CREATE TABLE [DietPlanItems] (
    [Id] int NOT NULL IDENTITY,
    [DietPlanId] int NOT NULL,
    [DayNumber] int NOT NULL DEFAULT 1,
    [MealType] nvarchar(50) NOT NULL,
    [MealName] nvarchar(100) NOT NULL,
    [Calories] int NOT NULL DEFAULT 0,
    [ProteinMacros] nvarchar(100) NULL,
    [CarbMacros] nvarchar(100) NULL,
    [FatMacros] nvarchar(100) NULL,
    [Ingredients] nvarchar(500) NULL,
    [Notes] nvarchar(500) NULL,
    [IsActive] bit NOT NULL,
    CONSTRAINT [PK_DietPlanItems] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_DietPlanItems_DietPlans_DietPlanId] FOREIGN KEY ([DietPlanId]) REFERENCES [DietPlans] ([Id]) ON DELETE CASCADE
);

CREATE TABLE [Memberships] (
    [Id] int NOT NULL IDENTITY,
    [MembershipType] nvarchar(50) NOT NULL,
    [Price] decimal(10,2) NOT NULL,
    [DurationInMonths] int NOT NULL,
    [HasDietPlan] bit NOT NULL,
    [HasWorkoutPlan] bit NOT NULL,
    [HasNotification] bit NULL,
    [IsFollowedByTrainer] bit NULL,
    [CancellationDate] datetime2 NULL,
    [CancellationReason] nvarchar(500) NULL,
    [PreferredTrainingTime] nvarchar(50) NULL,
    [TrainingIntensity] nvarchar(20) NULL,
    [DietPlanId] int NULL,
    [WorkoutPlanId] int NULL,
    [DietPlanId1] int NULL,
    [IsActive] bit NOT NULL DEFAULT CAST(1 AS bit),
    CONSTRAINT [PK_Memberships] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_Memberships_DietPlans_DietPlanId] FOREIGN KEY ([DietPlanId]) REFERENCES [DietPlans] ([Id]) ON DELETE SET NULL,
    CONSTRAINT [FK_Memberships_DietPlans_DietPlanId1] FOREIGN KEY ([DietPlanId1]) REFERENCES [DietPlans] ([Id]),
    CONSTRAINT [FK_Memberships_WorkoutPlans_WorkoutPlanId] FOREIGN KEY ([WorkoutPlanId]) REFERENCES [WorkoutPlans] ([Id]) ON DELETE SET NULL
);

CREATE TABLE [WorkoutPlanItems] (
    [Id] int NOT NULL IDENTITY,
    [WorkoutPlanId] int NOT NULL,
    [DayNumber] int NOT NULL DEFAULT 1,
    [ExerciseName] nvarchar(100) NOT NULL,
    [Sets] nvarchar(50) NULL,
    [Reps] nvarchar(50) NULL,
    [RestDuration] nvarchar(50) NULL,
    [Equipment] nvarchar(100) NULL,
    [VideoUrl] nvarchar(500) NULL,
    [Notes] nvarchar(500) NULL,
    [IsActive] bit NOT NULL,
    CONSTRAINT [PK_WorkoutPlanItems] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_WorkoutPlanItems_WorkoutPlans_WorkoutPlanId] FOREIGN KEY ([WorkoutPlanId]) REFERENCES [WorkoutPlans] ([Id]) ON DELETE CASCADE
);

CREATE TABLE [DietPlanAssignments] (
    [Id] int NOT NULL IDENTITY,
    [DietPlanId] int NOT NULL,
    [StartDate] date NOT NULL,
    [EndDate] date NOT NULL,
    [MemberId] int NOT NULL,
    [MemberId1] nvarchar(450) NOT NULL,
    [IsActive] bit NOT NULL DEFAULT CAST(1 AS bit),
    CONSTRAINT [PK_DietPlanAssignments] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_DietPlanAssignments_DietPlans_DietPlanId] FOREIGN KEY ([DietPlanId]) REFERENCES [DietPlans] ([Id]) ON DELETE NO ACTION,
    CONSTRAINT [FK_DietPlanAssignments_Members_MemberId1] FOREIGN KEY ([MemberId1]) REFERENCES [Members] ([Id]) ON DELETE CASCADE
);

CREATE TABLE [Payments] (
    [Id] int NOT NULL IDENTITY,
    [MemberId] nvarchar(450) NOT NULL,
    [PaymentType] nvarchar(50) NOT NULL,
    [Description] nvarchar(100) NOT NULL,
    [Amount] decimal(10,2) NOT NULL,
    [Currency] nvarchar(20) NOT NULL,
    [PaymentMethod] nvarchar(20) NOT NULL,
    [TransactionId] nvarchar(50) NULL,
    [Status] nvarchar(20) NOT NULL,
    [PaymentDate] datetime2 NOT NULL,
    [DueDate] datetime2 NULL,
    [ProcessedDate] datetime2 NULL,
    [Notes] nvarchar(500) NULL,
    [BillingName] nvarchar(200) NULL,
    [BillingAddress] nvarchar(500) NULL,
    [BillingEmail] nvarchar(100) NULL,
    [IsActive] bit NOT NULL,
    CONSTRAINT [PK_Payments] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_Payments_Members_MemberId] FOREIGN KEY ([MemberId]) REFERENCES [Members] ([Id]) ON DELETE CASCADE
);

CREATE TABLE [WorkoutAssignments] (
    [Id] int NOT NULL IDENTITY,
    [WorkoutPlanId] int NOT NULL,
    [MemberId] nvarchar(450) NOT NULL,
    [StartDate] date NOT NULL,
    [EndDate] date NOT NULL,
    [IsActive] bit NOT NULL DEFAULT CAST(1 AS bit),
    CONSTRAINT [PK_WorkoutAssignments] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_WorkoutAssignments_Members_MemberId] FOREIGN KEY ([MemberId]) REFERENCES [Members] ([Id]) ON DELETE CASCADE,
    CONSTRAINT [FK_WorkoutAssignments_WorkoutPlans_WorkoutPlanId] FOREIGN KEY ([WorkoutPlanId]) REFERENCES [WorkoutPlans] ([Id]) ON DELETE NO ACTION
);

CREATE TABLE [MealLogs] (
    [Id] int NOT NULL IDENTITY,
    [DietPlanAssignmentId] int NOT NULL,
    [Date] date NOT NULL DEFAULT (GETUTCDATE()),
    [MealsConsumed] nvarchar(max) NOT NULL,
    [Notes] nvarchar(1000) NULL,
    [CaloriesConsumed] int NULL DEFAULT 0,
    [IsActive] bit NOT NULL,
    CONSTRAINT [PK_MealLogs] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_MealLogs_DietPlanAssignments_DietPlanAssignmentId] FOREIGN KEY ([DietPlanAssignmentId]) REFERENCES [DietPlanAssignments] ([Id]) ON DELETE CASCADE
);

CREATE INDEX [IX_AspNetRoleClaims_RoleId] ON [AspNetRoleClaims] ([RoleId]);

CREATE UNIQUE INDEX [RoleNameIndex] ON [AspNetRoles] ([NormalizedName]) WHERE [NormalizedName] IS NOT NULL;

CREATE INDEX [IX_AspNetUserClaims_UserId] ON [AspNetUserClaims] ([UserId]);

CREATE INDEX [IX_AspNetUserLogins_UserId] ON [AspNetUserLogins] ([UserId]);

CREATE INDEX [IX_AspNetUserRoles_RoleId] ON [AspNetUserRoles] ([RoleId]);

CREATE INDEX [EmailIndex] ON [AspNetUsers] ([NormalizedEmail]);

CREATE INDEX [IX_AspNetUsers_Email] ON [AspNetUsers] ([Email]);

CREATE INDEX [IX_AspNetUsers_FullName] ON [AspNetUsers] ([FullName]);

CREATE UNIQUE INDEX [UserNameIndex] ON [AspNetUsers] ([NormalizedUserName]) WHERE [NormalizedUserName] IS NOT NULL;

CREATE INDEX [IX_DietPlanAssignments_DietPlanId] ON [DietPlanAssignments] ([DietPlanId]);

CREATE INDEX [IX_DietPlanAssignments_EndDate] ON [DietPlanAssignments] ([EndDate]);

CREATE INDEX [IX_DietPlanAssignments_IsActive] ON [DietPlanAssignments] ([IsActive]);

CREATE INDEX [IX_DietPlanAssignments_MemberId] ON [DietPlanAssignments] ([MemberId]);

CREATE INDEX [IX_DietPlanAssignments_MemberId_IsActive] ON [DietPlanAssignments] ([MemberId], [IsActive]);

CREATE INDEX [IX_DietPlanAssignments_MemberId1] ON [DietPlanAssignments] ([MemberId1]);

CREATE INDEX [IX_DietPlanAssignments_StartDate] ON [DietPlanAssignments] ([StartDate]);

CREATE INDEX [IX_DietPlanItems_DietPlanId] ON [DietPlanItems] ([DietPlanId]);

CREATE INDEX [IX_DietPlanItems_DietPlanId_DayNumber] ON [DietPlanItems] ([DietPlanId], [DayNumber]);

CREATE UNIQUE INDEX [IX_DietPlanItems_DietPlanId_DayNumber_MealType_MealName] ON [DietPlanItems] ([DietPlanId], [DayNumber], [MealType], [MealName]);

CREATE INDEX [IX_DietPlanItems_MealType] ON [DietPlanItems] ([MealType]);

CREATE INDEX [IX_DietPlans_IsActive] ON [DietPlans] ([IsActive]);

CREATE INDEX [IX_DietPlans_Name] ON [DietPlans] ([Name]);

CREATE INDEX [IX_DietPlans_TotalCalories] ON [DietPlans] ([TotalCalories]);

CREATE INDEX [IX_MealLogs_Date] ON [MealLogs] ([Date]);

CREATE INDEX [IX_MealLogs_DietPlanAssignmentId] ON [MealLogs] ([DietPlanAssignmentId]);

CREATE UNIQUE INDEX [IX_MealLogs_DietPlanAssignmentId_Date] ON [MealLogs] ([DietPlanAssignmentId], [Date]);

CREATE INDEX [IX_Members_JoinDate] ON [Members] ([JoinDate]);

CREATE INDEX [IX_Memberships_DietPlanId] ON [Memberships] ([DietPlanId]);

CREATE INDEX [IX_Memberships_DietPlanId1] ON [Memberships] ([DietPlanId1]);

CREATE INDEX [IX_Memberships_MembershipType] ON [Memberships] ([MembershipType]);

CREATE INDEX [IX_Memberships_WorkoutPlanId] ON [Memberships] ([WorkoutPlanId]);

CREATE INDEX [IX_Notifications_DeliveryMethod] ON [Notifications] ([DeliveryMethod]);

CREATE INDEX [IX_Notifications_SendTime] ON [Notifications] ([SendTime]);

CREATE INDEX [IX_Notifications_Status] ON [Notifications] ([Status]);

CREATE INDEX [IX_Notifications_Type] ON [Notifications] ([Type]);

CREATE INDEX [IX_Notifications_UserId] ON [Notifications] ([UserId]);

CREATE INDEX [IX_Notifications_UserId_Status_SendTime] ON [Notifications] ([UserId], [Status], [SendTime]);

CREATE INDEX [IX_Payments_MemberId] ON [Payments] ([MemberId]);

CREATE INDEX [IX_WorkoutAssignments_EndDate] ON [WorkoutAssignments] ([EndDate]);

CREATE INDEX [IX_WorkoutAssignments_IsActive] ON [WorkoutAssignments] ([IsActive]);

CREATE INDEX [IX_WorkoutAssignments_MemberId] ON [WorkoutAssignments] ([MemberId]);

CREATE INDEX [IX_WorkoutAssignments_MemberId_IsActive] ON [WorkoutAssignments] ([MemberId], [IsActive]);

CREATE INDEX [IX_WorkoutAssignments_StartDate] ON [WorkoutAssignments] ([StartDate]);

CREATE INDEX [IX_WorkoutAssignments_WorkoutPlanId] ON [WorkoutAssignments] ([WorkoutPlanId]);

CREATE INDEX [IX_WorkoutPlanItems_WorkoutPlanId] ON [WorkoutPlanItems] ([WorkoutPlanId]);

CREATE INDEX [IX_WorkoutPlanItems_WorkoutPlanId_DayNumber] ON [WorkoutPlanItems] ([WorkoutPlanId], [DayNumber]);

CREATE UNIQUE INDEX [IX_WorkoutPlanItems_WorkoutPlanId_DayNumber_ExerciseName] ON [WorkoutPlanItems] ([WorkoutPlanId], [DayNumber], [ExerciseName]);

CREATE INDEX [IX_WorkoutPlans_IsActive] ON [WorkoutPlans] ([IsActive]);

CREATE INDEX [IX_WorkoutPlans_Name] ON [WorkoutPlans] ([Name]);

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20251126203943_FixMealLogRelationships', N'9.0.10');

ALTER TABLE [Payments] DROP CONSTRAINT [FK_Payments_Members_MemberId];

ALTER TABLE [WorkoutPlans] ADD [CreatedAt] datetime2 NOT NULL DEFAULT '0001-01-01T00:00:00.0000000';

ALTER TABLE [WorkoutPlans] ADD [DeletedAt] datetime2 NULL;

ALTER TABLE [WorkoutPlans] ADD [DurationWeeks] int NOT NULL DEFAULT 0;

ALTER TABLE [WorkoutPlans] ADD [UpdatedAt] datetime2 NOT NULL DEFAULT '0001-01-01T00:00:00.0000000';

ALTER TABLE [WorkoutPlanItems] ADD [CreatedAt] datetime2 NOT NULL DEFAULT '0001-01-01T00:00:00.0000000';

ALTER TABLE [WorkoutPlanItems] ADD [DeletedAt] datetime2 NULL;

ALTER TABLE [WorkoutPlanItems] ADD [UpdatedAt] datetime2 NOT NULL DEFAULT '0001-01-01T00:00:00.0000000';

ALTER TABLE [WorkoutAssignments] ADD [CreatedAt] datetime2 NOT NULL DEFAULT '0001-01-01T00:00:00.0000000';

ALTER TABLE [WorkoutAssignments] ADD [DeletedAt] datetime2 NULL;

ALTER TABLE [WorkoutAssignments] ADD [UpdatedAt] datetime2 NOT NULL DEFAULT '0001-01-01T00:00:00.0000000';

DECLARE @var sysname;
SELECT @var = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Payments]') AND [c].[name] = N'TransactionId');
IF @var IS NOT NULL EXEC(N'ALTER TABLE [Payments] DROP CONSTRAINT [' + @var + '];');
ALTER TABLE [Payments] ALTER COLUMN [TransactionId] nvarchar(100) NULL;

DECLARE @var1 sysname;
SELECT @var1 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Payments]') AND [c].[name] = N'PaymentMethod');
IF @var1 IS NOT NULL EXEC(N'ALTER TABLE [Payments] DROP CONSTRAINT [' + @var1 + '];');
ALTER TABLE [Payments] ALTER COLUMN [PaymentMethod] nvarchar(50) NOT NULL;

ALTER TABLE [Payments] ADD [CreatedAt] datetime2 NOT NULL DEFAULT '0001-01-01T00:00:00.0000000';

ALTER TABLE [Payments] ADD [DeletedAt] datetime2 NULL;

ALTER TABLE [Payments] ADD [UpdatedAt] datetime2 NOT NULL DEFAULT '0001-01-01T00:00:00.0000000';

ALTER TABLE [Notifications] ADD [CreatedAt] datetime2 NOT NULL DEFAULT '0001-01-01T00:00:00.0000000';

ALTER TABLE [Notifications] ADD [DeletedAt] datetime2 NULL;

ALTER TABLE [Notifications] ADD [UpdatedAt] datetime2 NOT NULL DEFAULT '0001-01-01T00:00:00.0000000';

ALTER TABLE [Memberships] ADD [CreatedAt] datetime2 NOT NULL DEFAULT '0001-01-01T00:00:00.0000000';

ALTER TABLE [Memberships] ADD [DeletedAt] datetime2 NULL;

ALTER TABLE [Memberships] ADD [Features] nvarchar(250) NULL;

ALTER TABLE [Memberships] ADD [UpdatedAt] datetime2 NOT NULL DEFAULT '0001-01-01T00:00:00.0000000';

ALTER TABLE [MealLogs] ADD [CreatedAt] datetime2 NOT NULL DEFAULT '0001-01-01T00:00:00.0000000';

ALTER TABLE [MealLogs] ADD [DeletedAt] datetime2 NULL;

ALTER TABLE [MealLogs] ADD [UpdatedAt] datetime2 NOT NULL DEFAULT '0001-01-01T00:00:00.0000000';

DECLARE @var2 sysname;
SELECT @var2 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[DietPlans]') AND [c].[name] = N'Description');
IF @var2 IS NOT NULL EXEC(N'ALTER TABLE [DietPlans] DROP CONSTRAINT [' + @var2 + '];');
UPDATE [DietPlans] SET [Description] = N'' WHERE [Description] IS NULL;
ALTER TABLE [DietPlans] ALTER COLUMN [Description] nvarchar(1000) NOT NULL;
ALTER TABLE [DietPlans] ADD DEFAULT N'' FOR [Description];

ALTER TABLE [DietPlans] ADD [CreatedAt] datetime2 NOT NULL DEFAULT '0001-01-01T00:00:00.0000000';

ALTER TABLE [DietPlans] ADD [DeletedAt] datetime2 NULL;

ALTER TABLE [DietPlans] ADD [UpdatedAt] datetime2 NOT NULL DEFAULT '0001-01-01T00:00:00.0000000';

ALTER TABLE [DietPlanItems] ADD [CreatedAt] datetime2 NOT NULL DEFAULT '0001-01-01T00:00:00.0000000';

ALTER TABLE [DietPlanItems] ADD [DeletedAt] datetime2 NULL;

ALTER TABLE [DietPlanItems] ADD [UpdatedAt] datetime2 NOT NULL DEFAULT '0001-01-01T00:00:00.0000000';

ALTER TABLE [DietPlanAssignments] ADD [CreatedAt] datetime2 NOT NULL DEFAULT '0001-01-01T00:00:00.0000000';

ALTER TABLE [DietPlanAssignments] ADD [DeletedAt] datetime2 NULL;

ALTER TABLE [DietPlanAssignments] ADD [UpdatedAt] datetime2 NOT NULL DEFAULT '0001-01-01T00:00:00.0000000';

CREATE TABLE [Subscription] (
    [Id] int NOT NULL IDENTITY,
    [MemberId] nvarchar(450) NOT NULL,
    [MembershipId] int NOT NULL,
    [StartDate] datetime2 NOT NULL,
    [EndDate] datetime2 NOT NULL,
    [Status] int NOT NULL,
    [CreatedAt] datetime2 NOT NULL,
    [PaymentId] int NOT NULL,
    [CurrentWeight] float NOT NULL,
    [TargetWeight] float NOT NULL,
    [Height] float NOT NULL,
    CONSTRAINT [PK_Subscription] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_Subscription_Members_MemberId] FOREIGN KEY ([MemberId]) REFERENCES [Members] ([Id]) ON DELETE NO ACTION,
    CONSTRAINT [FK_Subscription_Memberships_MembershipId] FOREIGN KEY ([MembershipId]) REFERENCES [Memberships] ([Id]) ON DELETE NO ACTION,
    CONSTRAINT [FK_Subscription_Payments_PaymentId] FOREIGN KEY ([PaymentId]) REFERENCES [Payments] ([Id]) ON DELETE NO ACTION
);

CREATE INDEX [IX_Subscription_MemberId] ON [Subscription] ([MemberId]);

CREATE INDEX [IX_Subscription_MembershipId] ON [Subscription] ([MembershipId]);

CREATE INDEX [IX_Subscription_PaymentId] ON [Subscription] ([PaymentId]);

ALTER TABLE [Payments] ADD CONSTRAINT [FK_Payments_Members_MemberId] FOREIGN KEY ([MemberId]) REFERENCES [Members] ([Id]) ON DELETE NO ACTION;

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20251130185204_Add Base Entity', N'9.0.10');

DECLARE @var3 sysname;
SELECT @var3 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Subscription]') AND [c].[name] = N'CurrentWeight');
IF @var3 IS NOT NULL EXEC(N'ALTER TABLE [Subscription] DROP CONSTRAINT [' + @var3 + '];');
ALTER TABLE [Subscription] DROP COLUMN [CurrentWeight];

DECLARE @var4 sysname;
SELECT @var4 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Subscription]') AND [c].[name] = N'Height');
IF @var4 IS NOT NULL EXEC(N'ALTER TABLE [Subscription] DROP CONSTRAINT [' + @var4 + '];');
ALTER TABLE [Subscription] DROP COLUMN [Height];

DECLARE @var5 sysname;
SELECT @var5 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Subscription]') AND [c].[name] = N'TargetWeight');
IF @var5 IS NOT NULL EXEC(N'ALTER TABLE [Subscription] DROP CONSTRAINT [' + @var5 + '];');
ALTER TABLE [Subscription] DROP COLUMN [TargetWeight];

ALTER TABLE [Members] ADD [Age] int NOT NULL DEFAULT 0;

ALTER TABLE [Members] ADD [CurrentWeight] float NOT NULL DEFAULT 0.0E0;

ALTER TABLE [Members] ADD [HasCompletedProfile] bit NOT NULL DEFAULT CAST(0 AS bit);

ALTER TABLE [Members] ADD [Height] float NOT NULL DEFAULT 0.0E0;

ALTER TABLE [Members] ADD [TargetWeight] float NOT NULL DEFAULT 0.0E0;

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20251130201035_Refactor member', N'9.0.10');

ALTER TABLE [Members] ADD [Gender] bit NULL;

CREATE TABLE [FitnessGoals] (
    [Id] int NOT NULL IDENTITY,
    [GoalsName] nvarchar(max) NOT NULL,
    [GoalsDescription] nvarchar(max) NOT NULL,
    [MemberId] nvarchar(450) NULL,
    [SubscriptionId] int NULL,
    [IsActive] bit NOT NULL,
    [CreatedAt] datetime2 NOT NULL,
    [UpdatedAt] datetime2 NOT NULL,
    [DeletedAt] datetime2 NULL,
    CONSTRAINT [PK_FitnessGoals] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_FitnessGoals_Members_MemberId] FOREIGN KEY ([MemberId]) REFERENCES [Members] ([Id]),
    CONSTRAINT [FK_FitnessGoals_Subscription_SubscriptionId] FOREIGN KEY ([SubscriptionId]) REFERENCES [Subscription] ([Id])
);

CREATE INDEX [IX_FitnessGoals_MemberId] ON [FitnessGoals] ([MemberId]);

CREATE INDEX [IX_FitnessGoals_SubscriptionId] ON [FitnessGoals] ([SubscriptionId]);

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20251130205047_New Fitness goals', N'9.0.10');

ALTER TABLE [DietPlanAssignments] DROP CONSTRAINT [FK_DietPlanAssignments_Members_MemberId1];

ALTER TABLE [FitnessGoals] DROP CONSTRAINT [FK_FitnessGoals_Members_MemberId];

ALTER TABLE [FitnessGoals] DROP CONSTRAINT [FK_FitnessGoals_Subscription_SubscriptionId];

ALTER TABLE [Subscription] DROP CONSTRAINT [FK_Subscription_Members_MemberId];

ALTER TABLE [Subscription] DROP CONSTRAINT [FK_Subscription_Memberships_MembershipId];

ALTER TABLE [Subscription] DROP CONSTRAINT [FK_Subscription_Payments_PaymentId];

DROP INDEX [IX_FitnessGoals_MemberId] ON [FitnessGoals];

DROP INDEX [IX_FitnessGoals_SubscriptionId] ON [FitnessGoals];

DROP INDEX [IX_DietPlanAssignments_MemberId1] ON [DietPlanAssignments];

ALTER TABLE [Subscription] DROP CONSTRAINT [PK_Subscription];

DECLARE @var6 sysname;
SELECT @var6 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[FitnessGoals]') AND [c].[name] = N'MemberId');
IF @var6 IS NOT NULL EXEC(N'ALTER TABLE [FitnessGoals] DROP CONSTRAINT [' + @var6 + '];');
ALTER TABLE [FitnessGoals] DROP COLUMN [MemberId];

DECLARE @var7 sysname;
SELECT @var7 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[FitnessGoals]') AND [c].[name] = N'SubscriptionId');
IF @var7 IS NOT NULL EXEC(N'ALTER TABLE [FitnessGoals] DROP CONSTRAINT [' + @var7 + '];');
ALTER TABLE [FitnessGoals] DROP COLUMN [SubscriptionId];

DECLARE @var8 sysname;
SELECT @var8 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[DietPlanAssignments]') AND [c].[name] = N'MemberId1');
IF @var8 IS NOT NULL EXEC(N'ALTER TABLE [DietPlanAssignments] DROP CONSTRAINT [' + @var8 + '];');
ALTER TABLE [DietPlanAssignments] DROP COLUMN [MemberId1];

EXEC sp_rename N'[Subscription]', N'Subscriptions', 'OBJECT';

EXEC sp_rename N'[Subscriptions].[IX_Subscription_PaymentId]', N'IX_Subscriptions_PaymentId', 'INDEX';

EXEC sp_rename N'[Subscriptions].[IX_Subscription_MembershipId]', N'IX_Subscriptions_MembershipId', 'INDEX';

EXEC sp_rename N'[Subscriptions].[IX_Subscription_MemberId]', N'IX_Subscriptions_MemberId', 'INDEX';

ALTER TABLE [Members] ADD [FitnessGoalId] int NULL;

DROP INDEX [IX_DietPlanAssignments_MemberId] ON [DietPlanAssignments];
DROP INDEX [IX_DietPlanAssignments_MemberId_IsActive] ON [DietPlanAssignments];
DECLARE @var9 sysname;
SELECT @var9 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[DietPlanAssignments]') AND [c].[name] = N'MemberId');
IF @var9 IS NOT NULL EXEC(N'ALTER TABLE [DietPlanAssignments] DROP CONSTRAINT [' + @var9 + '];');
ALTER TABLE [DietPlanAssignments] ALTER COLUMN [MemberId] nvarchar(450) NOT NULL;
CREATE INDEX [IX_DietPlanAssignments_MemberId] ON [DietPlanAssignments] ([MemberId]);
CREATE INDEX [IX_DietPlanAssignments_MemberId_IsActive] ON [DietPlanAssignments] ([MemberId], [IsActive]);

DECLARE @var10 sysname;
SELECT @var10 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Subscriptions]') AND [c].[name] = N'Status');
IF @var10 IS NOT NULL EXEC(N'ALTER TABLE [Subscriptions] DROP CONSTRAINT [' + @var10 + '];');
ALTER TABLE [Subscriptions] ALTER COLUMN [Status] nvarchar(max) NOT NULL;

ALTER TABLE [Subscriptions] ADD CONSTRAINT [PK_Subscriptions] PRIMARY KEY ([Id]);

CREATE INDEX [IX_Members_FitnessGoalId] ON [Members] ([FitnessGoalId]);

ALTER TABLE [DietPlanAssignments] ADD CONSTRAINT [FK_DietPlanAssignments_Members_MemberId] FOREIGN KEY ([MemberId]) REFERENCES [Members] ([Id]) ON DELETE CASCADE;

ALTER TABLE [Members] ADD CONSTRAINT [FK_Members_FitnessGoals_FitnessGoalId] FOREIGN KEY ([FitnessGoalId]) REFERENCES [FitnessGoals] ([Id]);

ALTER TABLE [Subscriptions] ADD CONSTRAINT [FK_Subscriptions_Members_MemberId] FOREIGN KEY ([MemberId]) REFERENCES [Members] ([Id]) ON DELETE NO ACTION;

ALTER TABLE [Subscriptions] ADD CONSTRAINT [FK_Subscriptions_Memberships_MembershipId] FOREIGN KEY ([MembershipId]) REFERENCES [Memberships] ([Id]) ON DELETE NO ACTION;

ALTER TABLE [Subscriptions] ADD CONSTRAINT [FK_Subscriptions_Payments_PaymentId] FOREIGN KEY ([PaymentId]) REFERENCES [Payments] ([Id]) ON DELETE NO ACTION;

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20251207025901_Fixing Some unused entities', N'9.0.10');

ALTER TABLE [Subscriptions] DROP CONSTRAINT [FK_Subscriptions_Members_MemberId];

ALTER TABLE [Subscriptions] DROP CONSTRAINT [FK_Subscriptions_Memberships_MembershipId];

ALTER TABLE [Subscriptions] DROP CONSTRAINT [FK_Subscriptions_Payments_PaymentId];

ALTER TABLE [Subscriptions] DROP CONSTRAINT [PK_Subscriptions];

EXEC sp_rename N'[Subscriptions]', N'Subscription', 'OBJECT';

EXEC sp_rename N'[Subscription].[IX_Subscriptions_PaymentId]', N'IX_Subscription_PaymentId', 'INDEX';

EXEC sp_rename N'[Subscription].[IX_Subscriptions_MembershipId]', N'IX_Subscription_MembershipId', 'INDEX';

EXEC sp_rename N'[Subscription].[IX_Subscriptions_MemberId]', N'IX_Subscription_MemberId', 'INDEX';

ALTER TABLE [Subscription] ADD CONSTRAINT [PK_Subscription] PRIMARY KEY ([Id]);

ALTER TABLE [Subscription] ADD CONSTRAINT [FK_Subscription_Members_MemberId] FOREIGN KEY ([MemberId]) REFERENCES [Members] ([Id]) ON DELETE NO ACTION;

ALTER TABLE [Subscription] ADD CONSTRAINT [FK_Subscription_Memberships_MembershipId] FOREIGN KEY ([MembershipId]) REFERENCES [Memberships] ([Id]) ON DELETE NO ACTION;

ALTER TABLE [Subscription] ADD CONSTRAINT [FK_Subscription_Payments_PaymentId] FOREIGN KEY ([PaymentId]) REFERENCES [Payments] ([Id]) ON DELETE NO ACTION;

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20251207030255_Rename Entites', N'9.0.10');

ALTER TABLE [DietPlanAssignments] DROP CONSTRAINT [FK_DietPlanAssignments_Members_MemberId];

ALTER TABLE [Memberships] DROP CONSTRAINT [FK_Memberships_DietPlans_DietPlanId];

ALTER TABLE [Memberships] DROP CONSTRAINT [FK_Memberships_DietPlans_DietPlanId1];

ALTER TABLE [Memberships] DROP CONSTRAINT [FK_Memberships_WorkoutPlans_WorkoutPlanId];

ALTER TABLE [WorkoutAssignments] DROP CONSTRAINT [FK_WorkoutAssignments_Members_MemberId];

DROP INDEX [IX_WorkoutAssignments_MemberId_IsActive] ON [WorkoutAssignments];

DROP INDEX [IX_DietPlanAssignments_MemberId] ON [DietPlanAssignments];

DROP INDEX [IX_DietPlanAssignments_MemberId_IsActive] ON [DietPlanAssignments];

DECLARE @var11 sysname;
SELECT @var11 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[DietPlanAssignments]') AND [c].[name] = N'MemberId');
IF @var11 IS NOT NULL EXEC(N'ALTER TABLE [DietPlanAssignments] DROP CONSTRAINT [' + @var11 + '];');
ALTER TABLE [DietPlanAssignments] DROP COLUMN [MemberId];

EXEC sp_rename N'[Memberships].[WorkoutPlanId]', N'WorkoutAssignmentId', 'COLUMN';

EXEC sp_rename N'[Memberships].[DietPlanId1]', N'DietPlanAssignmentId', 'COLUMN';

EXEC sp_rename N'[Memberships].[IX_Memberships_WorkoutPlanId]', N'IX_Memberships_WorkoutAssignmentId', 'INDEX';

EXEC sp_rename N'[Memberships].[IX_Memberships_DietPlanId1]', N'IX_Memberships_DietPlanAssignmentId', 'INDEX';

DECLARE @var12 sysname;
SELECT @var12 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[WorkoutAssignments]') AND [c].[name] = N'MemberId');
IF @var12 IS NOT NULL EXEC(N'ALTER TABLE [WorkoutAssignments] DROP CONSTRAINT [' + @var12 + '];');
ALTER TABLE [WorkoutAssignments] ALTER COLUMN [MemberId] nvarchar(450) NULL;

ALTER TABLE [Memberships] ADD CONSTRAINT [FK_Memberships_DietPlanAssignments_DietPlanAssignmentId] FOREIGN KEY ([DietPlanAssignmentId]) REFERENCES [DietPlanAssignments] ([Id]) ON DELETE SET NULL;

ALTER TABLE [Memberships] ADD CONSTRAINT [FK_Memberships_DietPlans_DietPlanId] FOREIGN KEY ([DietPlanId]) REFERENCES [DietPlans] ([Id]);

ALTER TABLE [Memberships] ADD CONSTRAINT [FK_Memberships_WorkoutAssignments_WorkoutAssignmentId] FOREIGN KEY ([WorkoutAssignmentId]) REFERENCES [WorkoutAssignments] ([Id]) ON DELETE SET NULL;

ALTER TABLE [WorkoutAssignments] ADD CONSTRAINT [FK_WorkoutAssignments_Members_MemberId] FOREIGN KEY ([MemberId]) REFERENCES [Members] ([Id]);

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20251207084805_Update some Entities', N'9.0.10');

ALTER TABLE [Memberships] DROP CONSTRAINT [FK_Memberships_DietPlanAssignments_DietPlanAssignmentId];

ALTER TABLE [Memberships] DROP CONSTRAINT [FK_Memberships_WorkoutAssignments_WorkoutAssignmentId];

DROP INDEX [IX_Memberships_DietPlanAssignmentId] ON [Memberships];

DROP INDEX [IX_Memberships_WorkoutAssignmentId] ON [Memberships];

DECLARE @var13 sysname;
SELECT @var13 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Memberships]') AND [c].[name] = N'DietPlanAssignmentId');
IF @var13 IS NOT NULL EXEC(N'ALTER TABLE [Memberships] DROP CONSTRAINT [' + @var13 + '];');
ALTER TABLE [Memberships] DROP COLUMN [DietPlanAssignmentId];

DECLARE @var14 sysname;
SELECT @var14 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Memberships]') AND [c].[name] = N'WorkoutAssignmentId');
IF @var14 IS NOT NULL EXEC(N'ALTER TABLE [Memberships] DROP CONSTRAINT [' + @var14 + '];');
ALTER TABLE [Memberships] DROP COLUMN [WorkoutAssignmentId];

ALTER TABLE [Subscription] ADD [DietPlanAssignmentId] int NULL;

ALTER TABLE [Subscription] ADD [WorkoutAssignmentId] int NULL;

CREATE INDEX [IX_Subscription_DietPlanAssignmentId] ON [Subscription] ([DietPlanAssignmentId]);

CREATE INDEX [IX_Subscription_WorkoutAssignmentId] ON [Subscription] ([WorkoutAssignmentId]);

ALTER TABLE [Subscription] ADD CONSTRAINT [FK_Subscription_DietPlanAssignments_DietPlanAssignmentId] FOREIGN KEY ([DietPlanAssignmentId]) REFERENCES [DietPlanAssignments] ([Id]) ON DELETE SET NULL;

ALTER TABLE [Subscription] ADD CONSTRAINT [FK_Subscription_WorkoutAssignments_WorkoutAssignmentId] FOREIGN KEY ([WorkoutAssignmentId]) REFERENCES [WorkoutAssignments] ([Id]) ON DELETE SET NULL;

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20251207090416_Fix Subscription Entity Errors ', N'9.0.10');

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20251207100330_Fix Some errors', N'9.0.10');

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20251229232134_Fix the isuue in notification', N'9.0.10');

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20260101210802_AddWeightLogAndChatMessage', N'9.0.10');

CREATE TABLE [ChatMessages] (
    [Id] int NOT NULL IDENTITY,
    [SenderId] nvarchar(450) NOT NULL,
    [ReceiverId] nvarchar(450) NOT NULL,
    [Message] nvarchar(2000) NOT NULL,
    [Timestamp] datetime2 NOT NULL,
    [IsRead] bit NOT NULL,
    [AttachmentUrl] nvarchar(max) NULL,
    [AttachmentType] nvarchar(max) NULL,
    [IsActive] bit NOT NULL,
    [CreatedAt] datetime2 NOT NULL,
    [UpdatedAt] datetime2 NOT NULL,
    [DeletedAt] datetime2 NULL,
    CONSTRAINT [PK_ChatMessages] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_ChatMessages_AspNetUsers_ReceiverId] FOREIGN KEY ([ReceiverId]) REFERENCES [AspNetUsers] ([Id]),
    CONSTRAINT [FK_ChatMessages_AspNetUsers_SenderId] FOREIGN KEY ([SenderId]) REFERENCES [AspNetUsers] ([Id])
);

CREATE INDEX [IX_ChatMessages_ReceiverId] ON [ChatMessages] ([ReceiverId]);

CREATE INDEX [IX_ChatMessages_SenderId] ON [ChatMessages] ([SenderId]);

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20260101212829_AddMultimediaToChat', N'9.0.10');

ALTER TABLE [ChatMessages] ADD [IsDelivered] bit NOT NULL DEFAULT CAST(0 AS bit);

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20260101223009_AddIsDeliveredToChatMessage', N'9.0.10');

DECLARE @var15 sysname;
SELECT @var15 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[ChatMessages]') AND [c].[name] = N'Message');
IF @var15 IS NOT NULL EXEC(N'ALTER TABLE [ChatMessages] DROP CONSTRAINT [' + @var15 + '];');
ALTER TABLE [ChatMessages] ALTER COLUMN [Message] nvarchar(2000) NULL;

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20260101224115_MakeChatMessageMessageOptional', N'9.0.10');

ALTER TABLE [WorkoutAssignments] DROP CONSTRAINT [FK_WorkoutAssignments_Members_MemberId];

DROP INDEX [IX_WorkoutAssignments_MemberId] ON [WorkoutAssignments];

DECLARE @var16 sysname;
SELECT @var16 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[WorkoutAssignments]') AND [c].[name] = N'MemberId');
IF @var16 IS NOT NULL EXEC(N'ALTER TABLE [WorkoutAssignments] DROP CONSTRAINT [' + @var16 + '];');
ALTER TABLE [WorkoutAssignments] DROP COLUMN [MemberId];

ALTER TABLE [Members] ADD [ActivityLevel] nvarchar(max) NULL;

CREATE TABLE [TrainerReviews] (
    [Id] int NOT NULL IDENTITY,
    [TrainerId] nvarchar(450) NOT NULL,
    [MemberId] nvarchar(450) NOT NULL,
    [Rating] int NOT NULL,
    [Comment] nvarchar(500) NULL,
    [CreatedAt] datetime2 NOT NULL,
    CONSTRAINT [PK_TrainerReviews] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_TrainerReviews_AspNetUsers_MemberId] FOREIGN KEY ([MemberId]) REFERENCES [AspNetUsers] ([Id]) ON DELETE CASCADE,
    CONSTRAINT [FK_TrainerReviews_Trainers_TrainerId] FOREIGN KEY ([TrainerId]) REFERENCES [Trainers] ([Id]) ON DELETE NO ACTION
);

CREATE TABLE [WorkoutLogs] (
    [Id] int NOT NULL IDENTITY,
    [MemberId] nvarchar(450) NOT NULL,
    [WorkoutPlanId] int NULL,
    [Date] datetime2 NOT NULL,
    [DurationMinutes] int NOT NULL,
    [Notes] nvarchar(1000) NULL,
    [IsActive] bit NOT NULL,
    [CreatedAt] datetime2 NOT NULL,
    [UpdatedAt] datetime2 NOT NULL,
    [DeletedAt] datetime2 NULL,
    CONSTRAINT [PK_WorkoutLogs] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_WorkoutLogs_Members_MemberId] FOREIGN KEY ([MemberId]) REFERENCES [Members] ([Id]) ON DELETE CASCADE,
    CONSTRAINT [FK_WorkoutLogs_WorkoutPlans_WorkoutPlanId] FOREIGN KEY ([WorkoutPlanId]) REFERENCES [WorkoutPlans] ([Id])
);

CREATE TABLE [WorkoutLogEntries] (
    [Id] int NOT NULL IDENTITY,
    [WorkoutLogId] int NOT NULL,
    [WorkoutPlanItemId] int NULL,
    [ExerciseName] nvarchar(100) NOT NULL,
    [SetsPerformed] int NOT NULL,
    [RepsPerformed] nvarchar(50) NULL,
    [WeightLifted] nvarchar(50) NULL,
    CONSTRAINT [PK_WorkoutLogEntries] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_WorkoutLogEntries_WorkoutLogs_WorkoutLogId] FOREIGN KEY ([WorkoutLogId]) REFERENCES [WorkoutLogs] ([Id]) ON DELETE CASCADE,
    CONSTRAINT [FK_WorkoutLogEntries_WorkoutPlanItems_WorkoutPlanItemId] FOREIGN KEY ([WorkoutPlanItemId]) REFERENCES [WorkoutPlanItems] ([Id])
);

CREATE INDEX [IX_TrainerReviews_MemberId] ON [TrainerReviews] ([MemberId]);

CREATE INDEX [IX_TrainerReviews_TrainerId] ON [TrainerReviews] ([TrainerId]);

CREATE INDEX [IX_WorkoutLogEntries_WorkoutLogId] ON [WorkoutLogEntries] ([WorkoutLogId]);

CREATE INDEX [IX_WorkoutLogEntries_WorkoutPlanItemId] ON [WorkoutLogEntries] ([WorkoutPlanItemId]);

CREATE INDEX [IX_WorkoutLogs_MemberId] ON [WorkoutLogs] ([MemberId]);

CREATE INDEX [IX_WorkoutLogs_WorkoutPlanId] ON [WorkoutLogs] ([WorkoutPlanId]);

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20260103182019_add report and review', N'9.0.10');

ALTER TABLE [TrainerReviews] DROP CONSTRAINT [FK_TrainerReviews_AspNetUsers_MemberId];

ALTER TABLE [TrainerReviews] ADD CONSTRAINT [FK_TrainerReviews_Members_MemberId] FOREIGN KEY ([MemberId]) REFERENCES [Members] ([Id]) ON DELETE CASCADE;

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20260104135822_fix review joindate missing', N'9.0.10');

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20260104184803_AddWeightLog', N'9.0.10');

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20260104192146_AddWeightLogTable', N'9.0.10');

DROP INDEX [IX_MealLogs_DietPlanAssignmentId_Date] ON [MealLogs];

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20260104203148_fix the meal log issue with date ', N'9.0.10');

COMMIT;
GO

