import 'dart:convert';

import 'package:grooveon_mobile/config/api_config.dart';
import 'package:grooveon_mobile/helper/http_helper.dart';
import 'package:grooveon_mobile/models/subscription_response.dart';
import 'package:grooveon_mobile/providers/base_provider.dart';
import 'package:http/http.dart' as http;

class SubscriptionProvider extends BaseProvider<SubscriptionResponse> {
  SubscriptionProvider() : super("Subscription");

  @override
  SubscriptionResponse fromJson(dynamic json) {
    return SubscriptionResponse.fromJson(json);
  }

  Future<SubscriptionResponse?> getMyActive() async {
    final url = "${ApiConfig.apiBase}/api/Subscription/my-active";

    final response = await http.get(
      Uri.parse(url),
      headers: HttpHelper.getHeaders(),
    );

    HttpHelper.checkResponse(response);

    if (response.body.isEmpty || response.body == "null") {
      return null;
    }

    final data = jsonDecode(response.body);
    return SubscriptionResponse.fromJson(data);
  }

  Future<SubscriptionResponse?> getActiveByUserId(int userId) async {
    final url = "${ApiConfig.apiBase}/api/Subscription/active/$userId";

    final response = await http.get(
      Uri.parse(url),
      headers: HttpHelper.getHeaders(),
    );

    HttpHelper.checkResponse(response);

    if (response.body.isEmpty || response.body == "null") {
      return null;
    }

    final data = jsonDecode(response.body);
    return SubscriptionResponse.fromJson(data);
  }
}