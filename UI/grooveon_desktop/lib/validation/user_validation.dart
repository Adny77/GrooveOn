class Validators {

  static String? requiredField(String? value, String fieldName) {
    if (value == null || value.trim().isEmpty) {
      return "$fieldName is required";
    }
    return null;
  }

  static String? dateOfBirth(DateTime? value) {
    if (value == null) {
      return "Date of birth is required";
    }

    if (value.isAfter(DateTime.now())) {
      return "Date of birth is not valid";
    }

    return null;
  }

  static String? phone(String? value, {bool required = false}) {
  final v = value?.trim() ?? '';

  if (!required && v.isEmpty) return null;
  if (v.isEmpty) return "Phone number is required";

  // Dozvoljeni karakteri
  final allowedChars = RegExp(r'^[0-9+\-\s()]+$');
  if (!allowedChars.hasMatch(v)) {
    return "Enter a valid phone number";
  }

  // + može biti samo na početku
  if (v.contains('+') && !v.startsWith('+')) {
    return "The + sign can only be at the beginning";
  }

  // Izvuci samo cifre
  final digits = v.replaceAll(RegExp(r'\D'), '');

  // Lokalni brojevi
  if (digits.startsWith('060')) {
    if (digits.length != 10) {
      return "060 must have 7 digits after prefix";
    }
    return null;
  }

  if (digits.startsWith('061') || digits.startsWith('062')) {
    if (digits.length != 9) {
      return "061/062 must have 6 digits after prefix";
    }
    return null;
  }

  // Internacionalni format
  if (digits.startsWith('38760')) {
    if (digits.length != 12) {
      return "38760 must have 7 digits after prefix";
    }
    return null;
  }

  if (digits.startsWith('38761') || digits.startsWith('38762')) {
    if (digits.length != 11) {
      return "38761/38762 must have 6 digits after prefix";
    }
    return null;
  }

  return "Allowed formats: 060, 061, 062 or +387 / 387 variants";
}

  static String? username(String? value) {
    if (value == null || value.isEmpty) {
      return "Username is required";
    }

    final regex = RegExp(r'^(?=.*[0-9])[a-z0-9]{8,}$');

    if (!regex.hasMatch(value)) {
      return "Username must have at least 8 characters, lowercase letters, and at least one number";
    }

    return null;
  }

  static String? email(String? value) {
    if (value == null || value.isEmpty) {
      return "Email is required";
    }

    final regex = RegExp(
      r'^[\w-\.]+@([\w-]+\.)+[\w-]{2,4}$',
    );

    if (!regex.hasMatch(value)) {
      return "Email is not valid";
    }

    return null;
  }

  static String? password(String? value) {
    if (value == null || value.isEmpty) {
      return "Password is required";
    }

    final regex = RegExp(
      r'^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[\W_]).{8,}$',
    );

    if (!regex.hasMatch(value)) {
      return "Password must have:\n"
          "- at least 8 characters\n"
          "- one uppercase letter\n"
          "- one lowercase letter\n"
          "- one number\n"
          "- one special character";
    }

    return null;
  }

  static String? confirmPassword(String? value, String password) {
    if (value == null || value.isEmpty) {
      return "Repeat password";
    }

    if (value != password) {
      return "Passwords do not match";
    }

    return null;
  }
}
