class Validators {

  /// Ime / Prezime (obavezno)
  static String? requiredField(String? value, String fieldName) {
    if (value == null || value.trim().isEmpty) {
      return "$fieldName je obavezno";
    }
    return null;
  }

  /// Datum rodjenja (obavezno)
  static String? dateOfBirth(DateTime? value) {
    if (value == null) {
      return "Datum rođenja je obavezan";
    }

    if (value.isAfter(DateTime.now())) {
      return "Datum rođenja nije validan";
    }

    return null;
  }

  /// Username
  /// mala slova + bar jedan broj + min 8 karaktera
  static String? username(String? value) {
    if (value == null || value.isEmpty) {
      return "Username je obavezan";
    }

    final regex = RegExp(r'^(?=.*[0-9])[a-z0-9]{8,}$');

    if (!regex.hasMatch(value)) {
      return "Username mora imati min 8 karaktera, mala slova i bar jedan broj";
    }

    return null;
  }

  /// Email validacija
  static String? email(String? value) {
    if (value == null || value.isEmpty) {
      return "Email je obavezan";
    }

    final regex = RegExp(
      r'^[\w-\.]+@([\w-]+\.)+[\w-]{2,4}$',
    );

    if (!regex.hasMatch(value)) {
      return "Email nije validan";
    }

    return null;
  }

  /// Lozinka
  /// veliko slovo, malo slovo, broj, specijalni karakter, min 8
  static String? password(String? value) {
    if (value == null || value.isEmpty) {
      return "Lozinka je obavezna";
    }

    final regex = RegExp(
      r'^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[\W_]).{8,}$',
    );

    if (!regex.hasMatch(value)) {
      return "Lozinka mora imati:\n"
          "- najmanje 8 karaktera\n"
          "- jedno veliko slovo\n"
          "- jedno malo slovo\n"
          "- jedan broj\n"
          "- jedan specijalni karakter";
    }

    return null;
  }

  /// Ponovljena lozinka
  static String? confirmPassword(String? value, String password) {
    if (value == null || value.isEmpty) {
      return "Ponovite lozinku";
    }

    if (value != password) {
      return "Lozinke se ne podudaraju";
    }

    return null;
  }
}