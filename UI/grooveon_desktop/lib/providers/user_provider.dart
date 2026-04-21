import 'dart:convert';

import 'package:grooveon_desktop/config/api_config.dart';
import 'package:grooveon_desktop/helper/http_helper.dart';
import 'package:grooveon_desktop/models/response/user.dart';
import 'package:grooveon_desktop/providers/base_provider.dart';
import 'package:http/http.dart' as http;

class UserProvider extends BaseProvider<User> {
  UserProvider() : super("User");

  @override
  User fromJson(dynamic data) => User.fromJson(data);

  Future<void> forgotPassword(String email) async {
    final url = Uri.parse("${ApiConfig.apiBase}/api/User/forgot-password");

    final response = await http.post(
      url,
      headers: HttpHelper.getHeaders(withToken: false),
      body: jsonEncode({
        "email": email.trim(),
      }),
    );

    HttpHelper.checkResponse(response);
  }
}