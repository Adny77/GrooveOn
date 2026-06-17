import 'package:flutter/material.dart';
import 'package:grooveon_desktop/exception/ApiException.dart';
import 'package:grooveon_desktop/helper/exception_read_helper.dart';
import 'package:grooveon_desktop/routes/app_routes.dart';
import 'package:grooveon_desktop/utils/session.dart';
import 'package:http/http.dart' as http;

class HttpHelper {
  static final GlobalKey<NavigatorState> navigatorKey =
      GlobalKey<NavigatorState>();

  static void checkResponse(http.Response response) {
    if (response.statusCode >= 200 && response.statusCode < 300) {
      return;
    }

    final message = extractApiErrorMessage(response.body);

    if (response.statusCode == 401) {
      if (Session.isLoggedIn) {
        Session.logout();
        navigatorKey.currentState
            ?.pushNamedAndRemoveUntil(AppRoutes.login, (_) => false);
        throw ApiException("Session expired. Please sign in again.");
      }
      throw ApiException(message.isNotEmpty ? message : "Invalid credentials.");
    }

    if (response.statusCode == 403) {
      throw ApiException(message.isNotEmpty ? message : "Access denied.");
    }

    if (response.statusCode == 404) {
      throw ApiException(message.isNotEmpty ? message : "Resource not found.");
    }

    if (response.statusCode == 409) {
      throw ApiException(
          message.isNotEmpty ? message : "A record with the same data already exists.");
    }

    throw ApiException(message.isNotEmpty ? message : "An unexpected error occurred.");
  }

  static Map<String, String> getHeaders({bool withToken = true}) {
    final headers = {
      "Content-Type": "application/json",
      "Accept": "application/json",
    };

    if (withToken && Session.token != null) {
      headers["Authorization"] = "Bearer ${Session.token}";
    }

    return headers;
  }
}
