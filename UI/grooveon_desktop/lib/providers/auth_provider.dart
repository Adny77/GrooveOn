import 'dart:convert';

import 'package:flutter/material.dart';
import 'package:grooveon_desktop/config/api_config.dart';
import 'package:grooveon_desktop/helper/http_helper.dart';
import 'package:http/http.dart' as http;

import '../models/request/login_request.dart';
import '../models/response/login_response.dart';
import '../utils/session.dart';

class AuthProvider with ChangeNotifier {
  static  String apiUrl = "${ApiConfig.apiBase}/api/User/login";

  Future<String> login(LoginRequest request) async {
    final response = await http.post(
      Uri.parse(apiUrl),
      headers: HttpHelper.getHeaders(withToken: false),
      body: jsonEncode(request.toJson()),
    );

    HttpHelper.checkResponse(response);

    final data = jsonDecode(response.body);
    final loginResp = LoginResponse.fromJson(data);

    final allowedRoles = {'Admin'};
    final hasAccess =
        loginResp.roles.any((role) => allowedRoles.contains(role));

    if (!hasAccess) return "FORBIDDEN";

    Session.token = loginResp.token;
    Session.userId = loginResp.userId;
    Session.username = loginResp.userName;
    Session.roles = loginResp.roles;
    await Session.save();

    notifyListeners();

    return "OK";
  }
}