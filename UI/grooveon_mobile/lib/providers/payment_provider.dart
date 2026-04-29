import 'dart:convert';

import 'package:grooveon_mobile/config/api_config.dart';
import 'package:grooveon_mobile/helper/http_helper.dart';
import 'package:grooveon_mobile/models/payment_response.dart';
import 'package:http/http.dart' as http;
import 'package:grooveon_mobile/providers/base_provider.dart';

class PaymentProvider extends BaseProvider<PaymentResponse> {
  PaymentProvider() : super("Payment");

  @override
  PaymentResponse fromJson(dynamic json) {
    return PaymentResponse.fromJson(json);
  }

  Future<String> createNewIntent(Map<String, dynamic> request) async {
    final url = "${ApiConfig.apiBase}/api/Payment/create-new-intent";

    final response = await http.post(
      Uri.parse(url),
      headers: HttpHelper.getHeaders(), 
      body: jsonEncode(request),
    );

    HttpHelper.checkResponse(response);

    final data = jsonDecode(response.body);
    return data["clientSecret"];

  }
}