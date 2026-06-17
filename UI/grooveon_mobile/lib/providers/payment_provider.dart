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

  Future<({String clientSecret, int paymentId})> createNewIntent(
      Map<String, dynamic> request) async {
    final url = "${ApiConfig.apiBase}/api/Payment/create-new-intent";

    final response = await http.post(
      Uri.parse(url),
      headers: HttpHelper.getHeaders(),
      body: jsonEncode(request),
    );

    HttpHelper.checkResponse(response);

    final data = jsonDecode(response.body) as Map<String, dynamic>;
    return (
      clientSecret: data["clientSecret"] as String,
      paymentId: (data["paymentId"] as num).toInt(),
    );
  }

  Future<String> getMyStatus(int paymentId) async {
    final url = "${ApiConfig.apiBase}/api/Payment/my-status/$paymentId";

    final response = await http.get(
      Uri.parse(url),
      headers: HttpHelper.getHeaders(),
    );

    HttpHelper.checkResponse(response);

    final data = jsonDecode(response.body) as Map<String, dynamic>;
    return data["status"] as String;
  }
}