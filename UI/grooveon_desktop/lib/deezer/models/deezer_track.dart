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

  DeezerTrack copyWith({
  int? id,
  String? title,
  String? titleShort,
  String? titleVersion,
  String? link,
  int? duration,
  int? rank,
  bool? explicitLyrics,
  String? preview,
  DeezerArtist? artist,
  DeezerAlbum? album,
}) {
  return DeezerTrack(
    id: id ?? this.id,
    title: title ?? this.title,
    titleShort: titleShort ?? this.titleShort,
    titleVersion: titleVersion ?? this.titleVersion,
    link: link ?? this.link,
    duration: duration ?? this.duration,
    rank: rank ?? this.rank,
    explicitLyrics: explicitLyrics ?? this.explicitLyrics,
    preview: preview ?? this.preview,
    artist: artist ?? this.artist,
    album: album ?? this.album,
  );
}
}