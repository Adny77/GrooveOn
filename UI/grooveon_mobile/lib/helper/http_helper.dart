import 'dart:async';
import 'package:flutter/material.dart';
import 'package:grooveon_mobile/exceptions/ApiException.dart';
import 'package:grooveon_mobile/helper/exception_read_helper.dart';
import 'package:grooveon_mobile/routes/app_routes.dart';
import 'package:grooveon_mobile/utils/Session.dart';
import 'package:http/http.dart' as http;

class HttpHelper {
  static final GlobalKey<NavigatorState> navigatorKey =
      GlobalKey<NavigatorState>();

  static void checkResponse(http.Response response) {
    if (response.statusCode >= 200 && response.statusCode < 300) {
      return;
    }

    String message = extractApiErrorMessage(response.body);

    if (response.statusCode == 401) {
      unawaited(Session.logout());
      navigatorKey.currentState
          ?.pushNamedAndRemoveUntil(AppRoutes.login, (_) => false);
      throw ApiException("Session expired. Please sign in again.");
    }

    if (response.statusCode == 403) {
      throw ApiException(message.isNotEmpty ? message : "FORBIDDEN");
    }

    if (response.statusCode == 404) {
      throw ApiException(message.isNotEmpty ? message : "NOTFOUND");
    }

    throw ApiException(message);
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
