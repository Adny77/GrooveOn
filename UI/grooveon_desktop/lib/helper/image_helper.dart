import 'dart:io';

import 'package:flutter/material.dart';
import 'package:grooveon_desktop/config/api_config.dart';
import 'package:image_picker/image_picker.dart';


class ImageHelper {

  static final ImagePicker _picker = ImagePicker();
  
  List<String> fileNames = ["users", "properties"];


  static Future<File?> openImagePicker({
    ImageSource source = ImageSource.gallery,
    int imageQuality = 85,
  }) async {
    final XFile? picked = await _picker.pickImage(
      source: source,
      imageQuality: imageQuality,
    );

    if (picked == null) return null;

    return File(picked.path);
  }



  static bool isHttp(String imagePath) {
    if (imagePath.startsWith('http')) {
      return true;
    }

    return false;
  }

  static bool hasValidImage(String? value) {
    if (value == null) return false;
    final v = value.trim();
    if (v.isEmpty) return false;
    if (v.toLowerCase() == 'null') return false;
    return true;
  }


  static Widget userPlaceholder(String username, {double fontSize = 26}) {
    final letter = username.trim().isNotEmpty
        ? username.trim()[0].toUpperCase()
        : '?';

    return const Center(
      child: Icon(Icons.person, size: 46, color: Color(0xFF4A4A4A)),
    );
  }
}


