import 'dart:convert';
import 'package:flutter/material.dart';
import 'package:grooveon_desktop/config/api_config.dart';
import 'package:grooveon_desktop/models/subscription_analytics.dart';
import 'package:grooveon_desktop/models/user_growth_point.dart';
import 'package:grooveon_desktop/utils/session.dart';
import 'package:http/http.dart' as http;

class ReportProvider with ChangeNotifier {
  Future<SubscriptionAnalytics> getAnalytics({
    required int year,
    int? month,
  }) async {
    var url = "${ApiConfig.apiBase}/api/Report?year=$year";

    if (month != null) {
      url += "&month=$month";
    }

    final response = await http.get(
      Uri.parse(url),
      headers: {
        "Content-Type": "application/json",
        "Authorization": "Bearer ${Session.token}",
      },
    );

    if (response.statusCode < 200 || response.statusCode > 299) {
      throw Exception("Greška pri dohvatu analytics podataka.");
    }

    return SubscriptionAnalytics.fromJson(jsonDecode(response.body));
  }

  Future<List<UserGrowthPoint>> getUserGrowthByMonth({
    required int year,
  }) async {
    final url =
        "${ApiConfig.apiBase}/api/Report/user-growth-by-month?year=$year";

    final response = await http.get(
      Uri.parse(url),
      headers: {
        "Content-Type": "application/json",
        "Authorization": "Bearer ${Session.token}",
      },
    );

    if (response.statusCode < 200 || response.statusCode > 299) {
      throw Exception("Greška pri učitavanju user growth podataka.");
    }

    final data = jsonDecode(response.body) as List;
    return data.map((e) => UserGrowthPoint.fromJson(e)).toList();
  }
}