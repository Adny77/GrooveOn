import 'package:grooveon_desktop/deezer/models/deezer_artist.dart';
import 'package:grooveon_desktop/deezer/models/deezer_genre_response.dart';
import 'package:json_annotation/json_annotation.dart';

part 'deezer_album.g.dart';

@JsonSerializable()
class DeezerAlbum {
  final int id;
  final String title;
  final String? cover;

  @JsonKey(name: 'cover_small')
  final String? coverSmall;

  @JsonKey(name: 'cover_medium')
  final String? coverMedium;

  @JsonKey(name: 'cover_big')
  final String? coverBig;

  @JsonKey(name: 'cover_xl')
  final String? coverXl;

  @JsonKey(name: 'release_date')
  final String? releaseDate;

  final DeezerArtist? artist;

  @JsonKey(fromJson: _genresFromJson, defaultValue: [])
  final List<DeezerGenreResponse> genres;

  DeezerAlbum({
    required this.id,
    required this.title,
    this.cover,
    this.coverSmall,
    this.coverMedium,
    this.coverBig,
    this.coverXl,
    this.releaseDate,
    this.artist,
    this.genres = const [],
  });

  factory DeezerAlbum.fromJson(Map<String, dynamic> json) =>
      _$DeezerAlbumFromJson(json);

  Map<String, dynamic> toJson() => _$DeezerAlbumToJson(this);

  static List<DeezerGenreResponse> _genresFromJson(dynamic json) {
    if (json == null) return [];

    final data = json['data'] as List<dynamic>? ?? [];

    return data
        .map((e) => DeezerGenreResponse.fromJson(e as Map<String, dynamic>))
        .toList();
  }
}