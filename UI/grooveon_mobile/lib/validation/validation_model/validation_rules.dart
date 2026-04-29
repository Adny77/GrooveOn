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
      if (v.isEmpty) return 'Email is required.';

      final regex = RegExp(r'^[^\s@]+@[^\s@]+\.[^\s@]+$');
      if (!regex.hasMatch(v)) {
        return 'Enter a valid email.';
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
    if (v.isEmpty) return 'Username is required.';

    if (v.length < min || v.length > max) {
      return 'Username must be between $min and $max characters.';
    }

    if (v.contains(' ')) {
      return 'Username must not contain spaces.';
    }

    if (!RegExp(r'[a-zA-Z]').hasMatch(v)) {
      return 'Username must contain at least one letter.';
    }

    if (!RegExp(r'[0-9]').hasMatch(v)) {
      return 'Username must contain at least one number.';
    }

    return null;
  });
}

  static FieldRule phone(String field, String value, {bool required = false}) {
    return FieldRule(field, () {
      final v = value.trim();

      if (!required && v.isEmpty) return null;
      if (v.isEmpty) return 'Phone is required.';

      final allowedChars = RegExp(r'^[0-9+\-\s()]+$');
      if (!allowedChars.hasMatch(v)) {
        return 'Enter a valid phone number.';
      }

      if (v.contains('+') && !v.startsWith('+')) {
        return 'The + sign can only be at the beginning.';
      }

      final digits = v.replaceAll(RegExp(r'\D'), '');

      if (digits.startsWith('060')) {
        if (digits.length != 10) {
          return 'The 060 number must have 7 digits after the prefix.';
        }
        return null;
      }

      if (digits.startsWith('061') || digits.startsWith('062')) {
        if (digits.length != 9) {
          return 'The 061/062 number must have 6 digits after the prefix.';
        }
        return null;
      }

      if (digits.startsWith('38760')) {
        if (digits.length != 12) {
          return 'The 38760 number must have 7 digits after the prefix.';
        }
        return null;
      }

      if (digits.startsWith('38761') || digits.startsWith('38762')) {
        if (digits.length != 11) {
          return 'The 38761/38762 number must have 6 digits after the prefix.';
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
        return 'Password must have 8+ characters, uppercase, lowercase, a number, and a special character.';
      }

      return null;
    });
  }
}