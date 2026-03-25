// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'song_upsert_request.dart';

// **************************************************************************
// JsonSerializableGenerator
// **************************************************************************

SongUpsertRequest _$SongUpsertRequestFromJson(Map<String, dynamic> json) =>
    SongUpsertRequest(
      externalTrackId: json['externalTrackId'] as String,
      externalArtistId: json['externalArtistId'] as String?,
      externalAlbumId: json['externalAlbumId'] as String?,
      source: json['source'] as String? ?? "Deezer",
      title: json['title'] as String,
      artistName: json['artistName'] as String,
      artistPicture: json['artistPicture'] as String?,
      albumTitle: json['albumTitle'] as String?,
      durationSeconds: (json['durationSeconds'] as num?)?.toInt(),
      previewUrl: json['previewUrl'] as String?,
      coverUrl: json['coverUrl'] as String?,
      releaseDate: json['releaseDate'] == null
          ? null
          : DateTime.parse(json['releaseDate'] as String),
      genres:
          (json['genres'] as List<dynamic>?)
              ?.map(
                (e) => GenreUpsertRequest.fromJson(e as Map<String, dynamic>),
              )
              .toList() ??
          const [],
    );

Map<String, dynamic> _$SongUpsertRequestToJson(SongUpsertRequest instance) =>
    <String, dynamic>{
      'externalTrackId': instance.externalTrackId,
      'externalArtistId': instance.externalArtistId,
      'externalAlbumId': instance.externalAlbumId,
      'source': instance.source,
      'title': instance.title,
      'artistName': instance.artistName,
      'artistPicture': instance.artistPicture,
      'albumTitle': instance.albumTitle,
      'durationSeconds': instance.durationSeconds,
      'previewUrl': instance.previewUrl,
      'coverUrl': instance.coverUrl,
      'releaseDate': instance.releaseDate?.toIso8601String(),
      'genres': instance.genres,
    };
