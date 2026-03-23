import 'package:grooveon_desktop/deezer/models/deezer_album.dart';
import 'package:grooveon_desktop/deezer/models/deezer_artist.dart';
import 'package:json_annotation/json_annotation.dart';

part 'deezer_track.g.dart';

@JsonSerializable()
class DeezerTrack {
  final int id;
  final String title;

  @JsonKey(name: 'title_short')
  final String? titleShort;

  @JsonKey(name: 'title_version')
  final String? titleVersion;

  final String? link;
  final int? duration;
  final int? rank;

  @JsonKey(name: 'explicit_lyrics')
  final bool? explicitLyrics;

  final String? preview;
  final DeezerArtist? artist;
  final DeezerAlbum? album;

  DeezerTrack({
    required this.id,
    required this.title,
    this.titleShort,
    this.titleVersion,
    this.link,
    this.duration,
    this.rank,
    this.explicitLyrics,
    this.preview,
    this.artist,
    this.album,
  });

  factory DeezerTrack.fromJson(Map<String, dynamic> json) =>
      _$DeezerTrackFromJson(json);

  Map<String, dynamic> toJson() => _$DeezerTrackToJson(this);
}