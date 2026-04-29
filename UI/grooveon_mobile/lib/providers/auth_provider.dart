import 'dart:convert';
import 'package:flutter/material.dart';
import 'package:grooveon_mobile/config/api_config.dart';
import 'package:grooveon_mobile/helper/http_helper.dart';
import 'package:grooveon_mobile/models/login_request.dart';
import 'package:grooveon_mobile/models/login_response.dart';
import 'package:grooveon_mobile/utils/Session.dart';
import 'package:http/http.dart' as http;


class AuthProvider with ChangeNotifier {
  static String apiUrl = '${ApiConfig.apiBase}/api/User/login';

  Future<String> login(LoginRequest request) async {
    final url = Uri.parse(apiUrl);

    final response = await http.post(
      url,
      headers: HttpHelper.getHeaders(withToken: false),
      body: jsonEncode(request.toJson()),
    );

    HttpHelper.checkResponse(response);

    final data = jsonDecode(response.body);
    final loginResp = LoginResponse.fromJson(data);

    final allowedRoles = {'User'};

    final hasAccess = loginResp.roles.any(
      (role) => allowedRoles.contains(role),
    );

    if (!hasAccess) return "FORBIDDEN";

    Session.token = loginResp.token;
    Session.userId = loginResp.userId;
    Session.username = loginResp.userName;
    Session.roles = loginResp.roles;

    notifyListeners();

    return "OK";
  }
}