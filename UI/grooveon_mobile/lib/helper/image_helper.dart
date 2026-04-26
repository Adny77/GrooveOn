import 'dart:io';
import 'package:flutter/material.dart';
import 'package:image_picker/image_picker.dart';
import 'package:grooveon_mobile/config/api_config.dart';

class ImageHelper {
  static final ImagePicker _picker = ImagePicker();

  static const String usersFolder = "users";
  static const String playlistsFolder = "playlists";

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
    return imagePath.trim().toLowerCase().startsWith("http");
  }

  static bool hasValidImage(String? value) {
    if (value == null) return false;

    final v = value.trim();

    if (v.isEmpty) return false;
    if (v.toLowerCase() == "null") return false;

    return true;
  }

  static String? imageUrl(String? imagePath, String folderName) {
  if (!hasValidImage(imagePath)) return null;

  final path = imagePath!.trim();

  if (isHttp(path)) {
    return path;
  }

  final baseFolder = ApiConfig.imageFolders[folderName];

  if (baseFolder == null) return null;

  return "$baseFolder/$path";
}

  static String? userImageUrl(String? userImage) {
    return imageUrl(userImage, usersFolder);
  }

  static String? playlistImageUrl(String? playlistImage) {
    return imageUrl(playlistImage, playlistsFolder);
  }

  static Widget userPlaceholder(String username, {double size = 46}) {
    return Center(
      child: Icon(
        Icons.person,
        size: size,
        color: const Color(0xFF4A4A4A),
      ),
    );
  }

  static Widget playlistPlaceholder({double size = 46}) {
    return Center(
      child: Icon(
        Icons.queue_music_rounded,
        size: size,
        color: const Color(0xFF9C27B0),
      ),
    );
  }
}