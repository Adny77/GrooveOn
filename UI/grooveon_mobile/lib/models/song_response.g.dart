// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'song_response.dart';

// **************************************************************************
// JsonSerializableGenerator
// **************************************************************************

SongResponse _$SongResponseFromJson(Map<String, dynamic> json) => SongResponse(
  id: (json['id'] as num).toInt(),
  externalTrackId: json['externalTrackId'] as String?,
  source: json['source'] as String,
  title: json['title'] as String,
  artistId: (json['artistId'] as num).toInt(),
  artistName: json['artistName'] as String,
  albumId: (json['albumId'] as num?)?.toInt(),
  albumTitle: json['albumTitle'] as String?,
  genreId: (json['genreId'] as num?)?.toInt(),
  genreName: json['genreName'] as String?,
  durationSeconds: (json['durationSeconds'] as num).toInt(),
  previewUrl: json['previewUrl'] as String?,
  coverUrl: json['coverUrl'] as String?,
  releaseDate: json['releaseDate'] == null
      ? null
      : DateTime.parse(json['releaseDate'] as String),
  isActive: json['isActive'] as bool,
  lastSyncedAt: json['lastSyncedAt'] == null
      ? null
      : DateTime.parse(json['lastSyncedAt'] as String),
  createdAt: DateTime.parse(json['createdAt'] as String),
  whyRecommended: json['whyRecommended'] as String?,
);

Map<String, dynamic> _$SongResponseToJson(SongResponse instance) =>
    <String, dynamic>{
      'id': instance.id,
      'externalTrackId': instance.externalTrackId,
      'source': instance.source,
      'title': instance.title,
      'artistId': instance.artistId,
      'artistName': instance.artistName,
      'albumId': instance.albumId,
      'albumTitle': instance.albumTitle,
      'genreId': instance.genreId,
      'genreName': instance.genreName,
      'durationSeconds': instance.durationSeconds,
      'previewUrl': instance.previewUrl,
      'coverUrl': instance.coverUrl,
      'releaseDate': instance.releaseDate?.toIso8601String(),
      'isActive': instance.isActive,
      'lastSyncedAt': instance.lastSyncedAt?.toIso8601String(),
      'createdAt': instance.createdAt.toIso8601String(),
      'whyRecommended': instance.whyRecommended,
    };
