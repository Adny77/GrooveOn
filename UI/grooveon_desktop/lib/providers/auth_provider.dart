import 'dart:convert';
import 'package:flutter/material.dart';
import 'package:grooveon_desktop/config/api_config.dart';
import 'package:http/http.dart' as http;
import '../models/login_request.dart';
import '../models/login_response.dart';
import '../utils/session.dart';

class AuthProvider with ChangeNotifier {
  static const String apiUrl = "${ApiConfig.apiBase}/api/User/login";

  Future<String> prijava(LoginRequest request) async {
    final url = Uri.parse(apiUrl);

    final response = await http.post(
      url,
      headers: {
        "Content-Type": "application/json",
        "Accept": "application/json",
      },
      body: jsonEncode(request.toJson()),
    );

    print("STATUS: ${response.statusCode}");
    print("BODY: ${response.body}");

    if (response.statusCode == 200) {
      final data = jsonDecode(response.body);
      final loginResp = LoginResponse.fromJson(data);

      final allowedRoles = {'Admin'};
      final imaPristup = loginResp.roles.any((role) => allowedRoles.contains(role));
      
      if (!imaPristup) return "ZABRANJENO";

      Session.token = loginResp.token;
      Session.userId = loginResp.userId;
      Session.username = loginResp.userName;
      Session.roles = loginResp.roles;

      notifyListeners(); 

      return "OK";
    }

    if (response.statusCode == 401) return "NEISPRAVNO";
    if (response.statusCode == 403) return "ZABRANJENO";

    return "GRESKA";
  }
}
