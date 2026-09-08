namespace Application.Common;

/// <summary>موجودیت درخواست‌شده پیدا نشد → 404</summary>
public class NotFoundException : Exception
{
    public NotFoundException(string message) : base(message) { }
}

/// <summary>درخواست با قوانین کسب‌وکار سازگار نیست → 400</summary>
public class BusinessRuleException : Exception
{
    public BusinessRuleException(string message) : base(message) { }
}

/// <summary>کاربر اجازه دسترسی به این منبع را ندارد → 403</summary>
public class ForbiddenException : Exception
{
    public ForbiddenException(string message) : base(message) { }
}