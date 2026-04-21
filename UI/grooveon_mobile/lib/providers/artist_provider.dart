import 'package:flutter/material.dart';
import 'package:grooveon_mobile/models/artist_response.dart';
import 'base_provider.dart';

class ArtistProvider extends BaseProvider<Artist> {
  ArtistProvider() : super("Artist");

  @override
  Artist fromJson(data) {
    return Artist.fromJson(data);
  }
}