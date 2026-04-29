import 'dart:convert';

import 'package:flutter/material.dart';
import 'package:grooveon_desktop/config/api_config.dart';
import 'package:grooveon_desktop/helper/http_helper.dart';
import 'package:grooveon_desktop/models/response/income_by_month_response.dart';
import 'package:grooveon_desktop/models/response/music_overview_response.dart';
import 'package:grooveon_desktop/models/response/subscription_analytics.dart';
import 'package:grooveon_desktop/models/response/user_growth_point.dart';
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
      headers: HttpHelper.getHeaders(),
    );

    HttpHelper.checkResponse(response);

    return SubscriptionAnalytics.fromJson(jsonDecode(response.body));
  }

  Future<SubscriptionAnalytics> getNewUsersSubscriptionAnalytics({
    required int year,
    int? month,
  }) async {
    final uri = Uri.parse(
      '${ApiConfig.apiBase}/api/Report/new-users-subscription-analytics',
    ).replace(queryParameters: {
      "year": year.toString(),
      if (month != null) "month": month.toString(),
    });

    final response = await http.get(
      uri,
      headers: HttpHelper.getHeaders(),
    );

    HttpHelper.checkResponse(response);

    return SubscriptionAnalytics.fromJson(jsonDecode(response.body));
  }

  Future<MusicOverviewResponse> getMusicOverview({
    required String mode,
    required int userId,
    required int year,
    int? month,
    int take = 4,
  }) async {
    final uri = Uri.parse("${ApiConfig.apiBase}/api/Report/music-overview")
        .replace(queryParameters: {
      "mode": mode,
      "userId": userId.toString(),
      "year": year.toString(),
      "take": take.toString(),
      if (month != null) "month": month.toString(),
    });

    final response = await http.get(
      uri,
      headers: HttpHelper.getHeaders(),
    );

    HttpHelper.checkResponse(response);

    return MusicOverviewResponse.fromJson(jsonDecode(response.body));
  }

  Future<List<UserGrowthPoint>> getUserGrowthByMonth({
    required int year,
  }) async {
    final url =
        "${ApiConfig.apiBase}/api/Report/user-growth-by-month?year=$year";

    final response = await http.get(
      Uri.parse(url),
      headers: HttpHelper.getHeaders(),
    );

    HttpHelper.checkResponse(response);

    final data = jsonDecode(response.body) as List;
    return data.map((e) => UserGrowthPoint.fromJson(e)).toList();
  }

  Future<List<IncomeByMonthResponse>> getIncomeByMonth({
    required int year,
  }) async {
    final url = "${ApiConfig.apiBase}/api/Report/income-by-month?year=$year";

    final response = await http.get(
      Uri.parse(url),
      headers: HttpHelper.getHeaders(),
    );

    HttpHelper.checkResponse(response);

    final data = jsonDecode(response.body) as List;
    return data.map((e) => IncomeByMonthResponse.fromJson(e)).toList();
  }
}