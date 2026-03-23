// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'song_upsert_request.dart';

// **************************************************************************
// JsonSerializableGenerator
// **************************************************************************

SongUpsertRequest _$SongUpsertRequestFromJson(Map<String, dynamic> json) =>
    SongUpsertRequest(
      externalTrackId: json['externalTrackId'] as String,
      source: json['source'] as String? ?? "Deezer",
      title: json['title'] as String,
      artistName: json['artistName'] as String,
      albumTitle: json['albumTitle'] as String?,
      durationSeconds: (json['durationSeconds'] as num).toInt(),
      previewUrl: json['previewUrl'] as String?,
      coverUrl: json['coverUrl'] as String?,
      releaseDate: json['releaseDate'] == null
          ? null
          : DateTime.parse(json['releaseDate'] as String),
    );

Map<String, dynamic> _$SongUpsertRequestToJson(SongUpsertRequest instance) =>
    <String, dynamic>{
      'externalTrackId': instance.externalTrackId,
      'source': instance.source,
      'title': instance.title,
      'artistName': instance.artistName,
      'albumTitle': instance.albumTitle,
      'durationSeconds': instance.durationSeconds,
      'previewUrl': instance.previewUrl,
      'coverUrl': instance.coverUrl,
      'releaseDate': instance.releaseDate?.toIso8601String(),
    };
