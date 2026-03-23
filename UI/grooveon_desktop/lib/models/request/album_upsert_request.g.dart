// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'album_upsert_request.dart';

// **************************************************************************
// JsonSerializableGenerator
// **************************************************************************

AlbumUpsertRequest _$AlbumUpsertRequestFromJson(Map<String, dynamic> json) =>
    AlbumUpsertRequest(
      externalAlbumId: json['externalAlbumId'] as String,
      externalArtistId: json['externalArtistId'] as String?,
      source: json['source'] as String? ?? "Deezer",
      title: json['title'] as String,
      artistName: json['artistName'] as String,
      coverUrl: json['coverUrl'] as String?,
      description: json['description'] as String?,
      releaseDate: json['releaseDate'] == null
          ? null
          : DateTime.parse(json['releaseDate'] as String),
      tracks: (json['tracks'] as List<dynamic>)
          .map((e) => SongUpsertRequest.fromJson(e as Map<String, dynamic>))
          .toList(),
    );

Map<String, dynamic> _$AlbumUpsertRequestToJson(AlbumUpsertRequest instance) =>
    <String, dynamic>{
      'externalAlbumId': instance.externalAlbumId,
      'externalArtistId': instance.externalArtistId,
      'source': instance.source,
      'title': instance.title,
      'artistName': instance.artistName,
      'coverUrl': instance.coverUrl,
      'description': instance.description,
      'releaseDate': instance.releaseDate?.toIso8601String(),
      'tracks': instance.tracks.map((e) => e.toJson()).toList(),
    };
