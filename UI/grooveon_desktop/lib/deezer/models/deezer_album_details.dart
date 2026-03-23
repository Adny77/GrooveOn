import 'package:grooveon_desktop/deezer/models/deezer_artist.dart';
import 'package:grooveon_desktop/deezer/models/deezer_track.dart';
import 'package:json_annotation/json_annotation.dart';

part 'deezer_album_details.g.dart';

@JsonSerializable()
class DeezerAlbumDetails {
  final int id;
  final String title;
  final String? upc;
  final String? link;
  final String? share;
  final String? cover;

  @JsonKey(name: 'cover_small')
  final String? coverSmall;

  @JsonKey(name: 'cover_medium')
  final String? coverMedium;

  @JsonKey(name: 'cover_big')
  final String? coverBig;

  @JsonKey(name: 'cover_xl')
  final String? coverXl;

  @JsonKey(name: 'genre_id')
  final int? genreId;

  final String? label;

  @JsonKey(name: 'nb_tracks')
  final int? nbTracks;

  final int? duration;
  final int? fans;

  @JsonKey(name: 'release_date')
  final String? releaseDate;

  @JsonKey(name: 'record_type')
  final String? recordType;

  final bool? available;
  final DeezerArtist? artist;
  final DeezerTracksContainer tracks;

  DeezerAlbumDetails({
    required this.id,
    required this.title,
    this.upc,
    this.link,
    this.share,
    this.cover,
    this.coverSmall,
    this.coverMedium,
    this.coverBig,
    this.coverXl,
    this.genreId,
    this.label,
    this.nbTracks,
    this.duration,
    this.fans,
    this.releaseDate,
    this.recordType,
    this.available,
    this.artist,
    required this.tracks,
  });

  factory DeezerAlbumDetails.fromJson(Map<String, dynamic> json) =>
      _$DeezerAlbumDetailsFromJson(json);

  Map<String, dynamic> toJson() => _$DeezerAlbumDetailsToJson(this);
}

@JsonSerializable()
class DeezerTracksContainer {
  final List<DeezerTrack> data;

  DeezerTracksContainer({
    required this.data,
  });

  factory DeezerTracksContainer.fromJson(Map<String, dynamic> json) =>
      _$DeezerTracksContainerFromJson(json);

  Map<String, dynamic> toJson() => _$DeezerTracksContainerToJson(this);
}