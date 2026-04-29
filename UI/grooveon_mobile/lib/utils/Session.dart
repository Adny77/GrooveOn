class Session {
  static String? token;
  static int? userId;
  static String? username;
  static List<String> roles = [];

  static void logout() {
    token = null;
    userId = null;
    username = null;
    roles = [];
  }
}
