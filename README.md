# Prison Management System

## Layihə Haqqında

**Prison Management System**, ASP.NET Core Web API istifadə edərək hazırlanmış, həbsxana idarəetmə proseslərini rəqəmsallaşdıran və optimallaşdıran bir sistemdir. Layihə **N-Tier Architecture** əsasında qurulmuşdur və aşağıdakı əsas funksionallıqları təmin edir:

- Məhkumların qeydiyyatı və idarə olunması
- Kameraların və mühafizəçilərin idarə olunması
- Ziyarətlərin və cəzaların izlənməsi
- Statistik hesabatların yaradılması

## Texnologiyalar və Alətlər

- **ASP.NET Core Web API**
- **Entity Framework Core** (Code First yanaşması)
- **JWT (JSON Web Token)** ilə Authentication və Authorization
- **FluentValidation** ilə məlumatların doğrulanması
- **Serilog** ilə loglama
- **AutoMapper** ilə obyektlərin xəritələnməsi
- **xUnit** və **Moq** ilə Unit Testing
- **Swagger** ilə API sənədləşməsi

## Layihə Arxitekturası

Layihə aşağıdakı qatlara bölünmüşdür:

1. **Presentation Layer**: API controller-ləri və HTTP sorğularının işlənməsi
2. **Business Layer**: Biznes məntiqi və xidmətlər
3. **Data Access Layer**: Verilənlər bazası əməliyyatları və repository-lər

## Əsas Xüsusiyyətlər

### 🔐 Authentication və Authorization

- **Token-based authentication**: İstifadəçilər login olduqdan sonra JWT token əldə edir və bu token vasitəsilə digər API-lərə müraciət edə bilirlər.
- **Role-based authorization**: İstifadəçilərə müxtəlif rollar təyin edilə bilər və bu rollara əsasən API-lərə giriş icazələri idarə olunur.
- **401 Unauthorized**: Token təqdim edilmədikdə və ya etibarsız olduqda bu status kodu qaytarılır.

### ✅ Validasiya

- **FluentValidation** istifadə edilərək bütün giriş məlumatları yoxlanılır.
- **400 Bad Request**: Əgər daxil edilən məlumatlar düzgün deyilsə, bu status kodu və uyğun səhv mesajları qaytarılır.

### ⚙️ Global Exception Handling

- **Middleware** vasitəsilə bütün istisnalar qlobal səviyyədə idarə olunur.
- **Serilog** ilə bütün xətalar və hadisələr loglanır.

### 📊 Standart API Cavabları

Bütün API cavabları aşağıdakı formatda təqdim olunur:

```json
{
  "success": true,
  "message": "Əməliyyat uğurla yerinə yetirildi",
  "data": { ... }
}

