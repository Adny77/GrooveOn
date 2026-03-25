import 'package:json_annotation/json_annotation.dart';
import 'genre_stat_item_response.dart';
import 'music_stat_item_response.dart';

part 'music_overview_response.g.dart';

@JsonSerializable(explicitToJson: true)
class MusicOverviewResponse {
  final String mode;
  final int year;
  final int? month;

  final List<MusicStatItemResponse> mostPlayedSongs;
  final List<MusicStatItemResponse> leastPlayedSongs;

  final List<MusicStatItemResponse> mostPlayedAlbums;
  final List<MusicStatItemResponse> leastPlayedAlbums;

  final List<MusicStatItemResponse> mostPlayedArtists;
  final List<MusicStatItemResponse> leastPlayedArtists;

  final List<GenreStatItemResponse> trendingGenres;

  MusicOverviewResponse({
    required this.mode,
    required this.year,
    this.month,
    required this.mostPlayedSongs,
    required this.leastPlayedSongs,
    required this.mostPlayedAlbums,
    required this.leastPlayedAlbums,
    required this.mostPlayedArtists,
    required this.leastPlayedArtists,
    required this.trendingGenres,
  });

  factory MusicOverviewResponse.fromJson(Map<String, dynamic> json) =>
      _$MusicOverviewResponseFromJson(json);

  Map<String, dynamic> toJson() => _$MusicOverviewResponseToJson(this);
}