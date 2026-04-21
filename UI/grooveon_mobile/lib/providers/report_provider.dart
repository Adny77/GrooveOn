import 'dart:convert';
import 'package:flutter/material.dart';
import 'package:http/http.dart' as http;

import '../config/api_config.dart';
import '../helper/http_helper.dart';
import '../models/mobile_home_response.dart';

class ReportProvider with ChangeNotifier {
  static String baseUrl = '${ApiConfig.apiBase}/api/Report';

  Future<MobileHomeResponse> getMobileHome({
    int takeTracks = 4,
    int takeArtists = 8,
  }) async {
    final uri = Uri.parse(
      '$baseUrl/mobile-home?takeTracks=$takeTracks&takeArtists=$takeArtists',
    );

    final response = await http.get(
      uri,
      headers: HttpHelper.getHeaders(),
    );

    HttpHelper.checkResponse(response);

    final data = jsonDecode(response.body) as Map<String, dynamic>;
    return MobileHomeResponse.fromJson(data);
  }
}