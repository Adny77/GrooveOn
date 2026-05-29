import 'dart:convert';

import 'package:grooveon_mobile/config/api_config.dart';
import 'package:grooveon_mobile/helper/http_helper.dart';
import 'package:grooveon_mobile/models/notification_response.dart';
import 'package:http/http.dart' as http;

import 'base_provider.dart';

class NotificationProvider extends BaseProvider<NotificationResponse> {
  NotificationProvider() : super("Notification");

  @override
  NotificationResponse fromJson(data) {
    return NotificationResponse.fromJson(data);
  }

  Future<NotificationResponse> markAsRead(int id) async {
    final url = "${ApiConfig.apiBase}/api/Notification/$id/mark-read";

    final response = await http.put(
      Uri.parse(url),
      headers: HttpHelper.getHeaders(),
    );

    HttpHelper.checkResponse(response);
    return fromJson(jsonDecode(response.body));
  }
}
