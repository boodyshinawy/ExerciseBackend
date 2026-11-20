
namespace Project.Security;

public enum SeizureStatus
{
    Registration = 1,          // قيد التسجيل
    SubCommittee = 2,          // اللجنة الفرعية
    PermanentCommittee = 3,    // اللجنة الدائمة
    SalesEntity = 4,           // جهة البيع
    ServicesAgency = 5,        // جهاز الخدمات العامة
    PartiallyAllocated = 6,    // مخصص جزئياً
    FullyAllocated = 7,        // مخصص بالكامل
    FinanceAuthority = 8,      // هيئة شئون مالية
    Completed = 9              // مكتمل
}
