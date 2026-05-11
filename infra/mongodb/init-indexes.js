// Run against each MongoDB instance after first startup
// Usage: mongosh "mongodb://localhost:27117" --file init-indexes.js

// ── UserManagement DB ─────────────────────────────────────────────────────────
db = db.getSiblingDB("tips-steps-usermgmt");
db.users.createIndex({ email: 1 }, { unique: true });
db.users.createIndex({ keycloakId: 1 }, { unique: true });
db.users.createIndex({ role: 1, isActive: 1 });
db.users.createIndex({ governorateCode: 1 });
db.children.createIndex({ parentId: 1 });
db.children.createIndex({ dateOfBirth: 1 });
db.doctor_profiles.createIndex({ licenseNumber: 1 }, { unique: true });
db.doctor_profiles.createIndex({ isApproved: 1 });
print("UserManagement indexes created");

// ── Content DB ────────────────────────────────────────────────────────────────
db = db.getSiblingDB("tips-steps-content");
db.articles.createIndex({ section: 1, status: 1, publishedAt: -1 });
db.articles.createIndex({ status: 1 });
db.articles.createIndex({ tags: 1 });
db.articles.createIndex({ minAgeMonths: 1, maxAgeMonths: 1 });
db.articles.createIndex({ titleAr: "text", titleEn: "text", bodyAr: "text", bodyEn: "text" });
print("Content indexes created");

// ── GrowthMatrix DB ───────────────────────────────────────────────────────────
db = db.getSiblingDB("tips-steps-growth");
db.growth_skills.createIndex({ ageGroup: 1, category: 1 });
db.growth_skills.createIndex({ isActive: 1, orderIndex: 1 });
db.assessments.createIndex({ childId: 1, completedAt: -1 });
db.assessments.createIndex({ parentId: 1 });
db.assessments.createIndex({ scoreLevel: 1 });
print("GrowthMatrix indexes created");

// ── Q&A DB ────────────────────────────────────────────────────────────────────
db = db.getSiblingDB("tips-steps-qa");
db.questions.createIndex({ parentId: 1, status: 1 });
db.questions.createIndex({ assignedDoctorId: 1, status: 1 });
db.questions.createIndex({ category: 1, submittedAt: -1 });
print("QA indexes created");

// ── Analytics Read-Only DB ────────────────────────────────────────────────────
db = db.getSiblingDB("tips-steps-analytics-read");
db.dailyUserMetrics.createIndex({ date: -1 }, { unique: true });
db.contentEngagement.createIndex({ contentId: 1, date: -1 });
db.contentEngagement.createIndex({ section: 1, date: -1 });
print("Analytics read-only indexes created");

// ── HealthIntelligence DB ─────────────────────────────────────────────────────
db = db.getSiblingDB("tips-steps-health-intel");
db.governorateReports.createIndex({ governorateCode: 1, healthIndicator: 1, reportPeriod: -1 });
db.governorateReports.createIndex({ aggregateCount: 1 }); // k-anonymity filter
print("HealthIntelligence indexes created");

print("=== All MongoDB indexes initialized successfully ===");
