import 'package:grooveon_mobile/models/music_stat_item_response.dart';
import 'package:json_annotation/json_annotation.dart';

part 'mobile_home_response.g.dart';

@JsonSerializable()
class MobileHomeResponse {
  final MusicStatItemResponse? songOfTheDay;
  final List<MusicStatItemResponse> topTracks;
  final List<MusicStatItemResponse> topArtists;

  MobileHomeResponse({
    this.songOfTheDay,
    this.topTracks = const [],
    this.topArtists = const [],
  });

  factory MobileHomeResponse.fromJson(Map<String, dynamic> json) =>
      _$MobileHomeResponseFromJson(json);

  Map<String, dynamic> toJson() => _$MobileHomeResponseToJson(this);
}