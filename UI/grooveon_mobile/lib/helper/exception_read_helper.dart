import 'dart:convert';

import 'package:grooveon_mobile/exceptions/ApiException.dart';


String extractApiErrorMessage(String responseBody) {
  try {
    final data = jsonDecode(responseBody);

    if (data is Map<String, dynamic>) {
      if (data['message'] != null &&
          data['message'].toString().trim().isNotEmpty) {
        return data['message'].toString();
      }

      if (data['title'] != null &&
          data['title'].toString().trim().isNotEmpty) {
        return data['title'].toString();
      }

      if (data['errors'] is Map<String, dynamic>) {
        final errors = data['errors'] as Map<String, dynamic>;

        for (final entry in errors.entries) {
          final value = entry.value;

          if (value is List && value.isNotEmpty) {
            return value.first.toString();
          }

          if (value != null) {
            return value.toString();
          }
        }
      }
    }

    return "An error occurred.";
  } catch (_) {
    return "Server communication error.";
  }
}

String extractErrorMessage(dynamic e) {
  try {
    if (e is ApiException) {
      return e.message;
    }

    final message = e.toString();

    if (message.startsWith('Exception: ')) {
      return message.replaceFirst('Exception: ', '');
    }

    return message;
  } catch (_) {
    return "An error occurred.";
  }
}