import 'package:grooveon_mobile/validation/validation_model/validation_field_rule.dart';

class Rules {
  static FieldRule requiredText(String field, String value, String message) {
    return FieldRule(field, () => value.trim().isEmpty ? message : null);
  }

  static FieldRule minLength(
    String field,
    String value,
    int min,
    String message,
  ) {
    return FieldRule(field, () => value.trim().length < min ? message : null);
  }

  static FieldRule positiveNumber(String field, String value, String message) {
    return FieldRule(field, () {
      final v = double.tryParse(value.replaceAll(',', '.'));
      if (v == null || v <= 0) return message;
      return null;
    });
  }

  static FieldRule requiredDate(String field, DateTime? date, String message) {
    return FieldRule(field, () => date == null ? message : null);
  }

  static FieldRule email(String field, String value, {bool required = true}) {
    return FieldRule(field, () {
      final v = value.trim();

      if (!required && v.isEmpty) return null;
      if (v.isEmpty) return 'Email je obavezan.';

      final regex = RegExp(r'^[^\s@]+@[^\s@]+\.[^\s@]+$');
      if (!regex.hasMatch(v)) {
        return 'Unesi ispravan email.';
      }

      return null;
    });
  }

  static FieldRule username(
    String field,
    String value, {
    bool required = true,
    int min = 3,
    int max = 20,
  }) {
    return FieldRule(field, () {
      final v = value.trim();

      if (!required && v.isEmpty) return null;
      if (v.isEmpty) return 'Username je obavezan.';

      if (v.length < min || v.length > max) {
        return 'Username mora imati između $min i $max karaktera.';
      }

      if (v.contains(' ')) {
        return 'Username ne smije sadržavati razmake.';
      }

      return null;
    });
  }

  static FieldRule phone(String field, String value, {bool required = false}) {
    return FieldRule(field, () {
      final v = value.trim();

      if (!required && v.isEmpty) return null;
      if (v.isEmpty) return 'Telefon je obavezan.';

      final allowedChars = RegExp(r'^[0-9+\-\s()]+$');
      if (!allowedChars.hasMatch(v)) {
        return 'Unesi ispravan broj telefona.';
      }

      if (v.contains('+') && !v.startsWith('+')) {
        return 'Znak + može biti samo na početku.';
      }

      final digits = v.replaceAll(RegExp(r'\D'), '');

      if (digits.startsWith('060')) {
        if (digits.length != 10) {
          return 'Broj 060 mora imati 7 cifara nakon pozivnog.';
        }
        return null;
      }

      if (digits.startsWith('061') || digits.startsWith('062')) {
        if (digits.length != 9) {
          return 'Broj 061/062 mora imati 6 cifara nakon pozivnog.';
        }
        return null;
      }

      if (digits.startsWith('38760')) {
        if (digits.length != 12) {
          return 'Broj 38760 mora imati 7 cifara nakon pozivnog.';
        }
        return null;
      }

      if (digits.startsWith('38761') || digits.startsWith('38762')) {
        if (digits.length != 11) {
          return 'Broj 38761/38762 mora imati 6 cifara nakon pozivnog.';
        }
        return null;
      }

      return 'Dozvoljeni su brojevi: 060, 061, 062 ili +387/387 varijante.';
    });
  }

  static FieldRule strongPassword(String field, String value) {
    return FieldRule(field, () {
      final v = value.trim();

      final hasMin = v.length >= 8;
      final hasUpper = RegExp(r'[A-Z]').hasMatch(v);
      final hasLower = RegExp(r'[a-z]').hasMatch(v);
      final hasDigit = RegExp(r'\d').hasMatch(v);
      final hasSpecial = RegExp(r'[!@#$%^&*(),.?":{}|<>]').hasMatch(v);

      if (!hasMin || !hasUpper || !hasLower || !hasDigit || !hasSpecial) {
        return 'Lozinka mora imati 8+ znakova, veliko, malo, broj i specijalni znak.';
      }

      return null;
    });
  }
}