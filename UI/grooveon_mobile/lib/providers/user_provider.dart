import 'dart:convert';
import 'package:grooveon_mobile/config/api_config.dart';
import 'package:grooveon_mobile/helper/http_helper.dart';
import 'package:grooveon_mobile/models/user.dart';
import 'package:http/http.dart' as http;

import 'base_provider.dart';

class UserProvider extends BaseProvider<User> {
  UserProvider() : super("User");

  @override
  User fromJson(dynamic data) {
    return User.fromJson(data);
  }

  Future<bool> forgotPassword(String email) async {
  final url = Uri.parse(
    "${ApiConfig.apiBase}/api/User/forgot-password",
  );

  final response = await http.post(
    url,
    headers: HttpHelper.getHeaders(),
    body: jsonEncode({
      "email": email,
    }),
  );

  HttpHelper.checkResponse(response);

  return true;
}

Future<bool> hasPremium() async {
  final url = Uri.parse(
    "${ApiConfig.apiBase}/api/User/current-user/has-premium",
  );

  final response = await http.get(
    url,
    headers: HttpHelper.getHeaders(),
  );

  HttpHelper.checkResponse(response);

  return response.body.toLowerCase() == "true";
}

Future<void> changePassword(Map<String, dynamic> request) async {
  final url = Uri.parse("${ApiConfig.apiBase}/api/User/change-password");

  final response = await http.post(
    url,
    headers: HttpHelper.getHeaders(),
    body: jsonEncode(request),
  );

  HttpHelper.checkResponse(response);
}

}