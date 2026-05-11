// Seed initial Growth Matrix skills into MongoDB
// mongosh "mongodb://localhost:27121" --file seed-growth-skills.js

db = db.getSiblingDB("tips-steps-growth");

const skills = [
  // ── 0-3 Months ──────────────────────────────────────────────
  { _id: "skill-001", ageGroup: 1, category: 1, skillType: 1, orderIndex: 1,
    nameAr: "يتابع الأشياء بعينيه", nameEn: "Tracks objects with eyes",
    descriptionAr: "يتابع الطفل جسماً متحركاً بعينيه لمسافة 20-30 سم",
    descriptionEn: "Child tracks a moving object with eyes at 20-30cm distance",
    weight: 1.0, isActive: true },
  { _id: "skill-002", ageGroup: 1, category: 3, skillType: 1, orderIndex: 2,
    nameAr: "يستجيب للأصوات", nameEn: "Responds to sounds",
    descriptionAr: "يدير رأسه نحو مصدر الصوت", descriptionEn: "Turns head toward sound source",
    weight: 1.0, isActive: true },
  { _id: "skill-003", ageGroup: 1, category: 2, skillType: 1, orderIndex: 3,
    nameAr: "يرفع رأسه أثناء الاستلقاء على البطن", nameEn: "Lifts head during tummy time",
    descriptionAr: "يرفع الطفل رأسه عند وضعه على بطنه", descriptionEn: "Child lifts head when placed on tummy",
    weight: 1.2, isActive: true },

  // ── 4-6 Months ──────────────────────────────────────────────
  { _id: "skill-010", ageGroup: 2, category: 2, skillType: 1, orderIndex: 1,
    nameAr: "يتدحرج من ظهره لبطنه", nameEn: "Rolls from back to tummy",
    weight: 1.2, isActive: true,
    descriptionAr: "يتدحرج الطفل من وضع الاستلقاء على ظهره إلى بطنه",
    descriptionEn: "Child rolls from back position to tummy" },
  { _id: "skill-011", ageGroup: 2, category: 3, skillType: 1, orderIndex: 2,
    nameAr: "يصدر أصواتاً (بابا، ماما)", nameEn: "Babbles (baba, mama)",
    weight: 1.0, isActive: true,
    descriptionAr: "يصدر الطفل أصواتاً مقطعية متكررة", descriptionEn: "Child produces repeated syllable sounds" },
  { _id: "skill-012", ageGroup: 2, category: 4, skillType: 1, orderIndex: 3,
    nameAr: "يبتسم للوجوه المألوفة", nameEn: "Smiles at familiar faces",
    weight: 1.0, isActive: true,
    descriptionAr: "يبتسم عند رؤية الوالدين أو مقدمي الرعاية",
    descriptionEn: "Smiles when seeing parents or caregivers" },

  // ── 7-12 Months ─────────────────────────────────────────────
  { _id: "skill-020", ageGroup: 3, category: 2, skillType: 1, orderIndex: 1,
    nameAr: "يجلس بدون دعم", nameEn: "Sits without support",
    weight: 1.3, isActive: true,
    descriptionAr: "يجلس الطفل بشكل مستقل دون حاجة للدعم",
    descriptionEn: "Child sits independently without support" },
  { _id: "skill-021", ageGroup: 3, category: 3, skillType: 2, orderIndex: 2,
    nameAr: "عدد الكلمات التي يستطيع الطفل قولها", nameEn: "Number of words child can say",
    weight: 1.0, maxValue: 5, isActive: true,
    descriptionAr: "كمية الكلمات المفهومة التي يقولها الطفل",
    descriptionEn: "Count of meaningful words child says" },
  { _id: "skill-022", ageGroup: 3, category: 1, skillType: 1, orderIndex: 3,
    nameAr: "يبحث عن شيء مخفي", nameEn: "Searches for hidden objects",
    weight: 1.1, isActive: true,
    descriptionAr: "يبحث عن لعبة تم إخفاؤها أمامه (ثبات الموضوع)",
    descriptionEn: "Searches for toy hidden in front of them (object permanence)" },

  // ── 1-2 Years ────────────────────────────────────────────────
  { _id: "skill-030", ageGroup: 4, category: 2, skillType: 1, orderIndex: 1,
    nameAr: "يمشي بمفرده", nameEn: "Walks independently",
    weight: 1.5, isActive: true,
    descriptionAr: "يمشي الطفل بخطوات مستقلة دون مساعدة",
    descriptionEn: "Child walks with independent steps without help" },
  { _id: "skill-031", ageGroup: 4, category: 3, skillType: 2, orderIndex: 2,
    nameAr: "عدد الكلمات في مفرداته", nameEn: "Vocabulary word count",
    weight: 1.0, maxValue: 20, isActive: true,
    descriptionAr: "عدد الكلمات التي يستخدمها الطفل بانتظام",
    descriptionEn: "Number of words child uses regularly" },
  { _id: "skill-032", ageGroup: 4, category: 5, skillType: 1, orderIndex: 3,
    nameAr: "يستخدم الملعقة للأكل", nameEn: "Uses spoon to eat",
    weight: 1.0, isActive: true,
    descriptionAr: "يحاول الطفل استخدام الملعقة بشكل مستقل",
    descriptionEn: "Child attempts to use spoon independently" },

  // ── 2-3 Years ────────────────────────────────────────────────
  { _id: "skill-040", ageGroup: 5, category: 3, skillType: 1, orderIndex: 1,
    nameAr: "يتحدث بجمل من كلمتين أو أكثر", nameEn: "Speaks in 2+ word sentences",
    weight: 1.3, isActive: true,
    descriptionAr: "يكوّن جملاً من كلمتين على الأقل مثل (أريد ماء)",
    descriptionEn: "Forms sentences of at least 2 words" },
  { _id: "skill-041", ageGroup: 5, category: 1, skillType: 1, orderIndex: 2,
    nameAr: "يرسم خطاً مستقيماً", nameEn: "Draws a straight line",
    weight: 1.0, isActive: true,
    descriptionAr: "يمسك القلم ويرسم خطاً مستقيماً", descriptionEn: "Holds pencil and draws a straight line" },
  { _id: "skill-042", ageGroup: 5, category: 4, skillType: 1, orderIndex: 3,
    nameAr: "يلعب مع أقرانه", nameEn: "Plays with peers",
    weight: 1.0, isActive: true,
    descriptionAr: "يتفاعل ويشارك في اللعب مع أطفال آخرين",
    descriptionEn: "Interacts and participates in play with other children" }
];

db.growth_skills.insertMany(skills);
print(`Inserted ${skills.length} growth skills`);
