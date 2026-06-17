import 'package:flutter_secure_storage/flutter_secure_storage.dart';

class Session {
  static const _storage = FlutterSecureStorage();

  static String? token;
  static int? userId;
  static String? username;
  static List<String> roles = [];

  static Future<void> save() async {
    await Future.wait([
      _storage.write(key: 'session_token', value: token),
      _storage.write(key: 'session_userId', value: userId?.toString()),
      _storage.write(key: 'session_username', value: username),
      _storage.write(key: 'session_roles', value: roles.join(',')),
    ]);
  }

  static Future<void> load() async {
    token = await _storage.read(key: 'session_token');
    final userIdStr = await _storage.read(key: 'session_userId');
    userId = userIdStr != null ? int.tryParse(userIdStr) : null;
    username = await _storage.read(key: 'session_username');
    final rolesStr = await _storage.read(key: 'session_roles');
    roles = (rolesStr != null && rolesStr.isNotEmpty)
        ? rolesStr.split(',').where((r) => r.isNotEmpty).toList()
        : [];
  }

  static bool get isLoggedIn => token != null && userId != null;

  static Future<void> logout() async {
    token = null;
    userId = null;
    username = null;
    roles = [];
    await _storage.deleteAll();
  }
}
