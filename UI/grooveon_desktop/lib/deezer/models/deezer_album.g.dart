// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'deezer_album.dart';

// **************************************************************************
// JsonSerializableGenerator
// **************************************************************************

DeezerAlbum _$DeezerAlbumFromJson(Map<String, dynamic> json) => DeezerAlbum(
  id: (json['id'] as num).toInt(),
  title: json['title'] as String,
  cover: json['cover'] as String?,
  coverSmall: json['cover_small'] as String?,
  coverMedium: json['cover_medium'] as String?,
  coverBig: json['cover_big'] as String?,
  coverXl: json['cover_xl'] as String?,
  releaseDate: json['release_date'] as String?,
  artist: json['artist'] == null
      ? null
      : DeezerArtist.fromJson(json['artist'] as Map<String, dynamic>),
);

Map<String, dynamic> _$DeezerAlbumToJson(DeezerAlbum instance) =>
    <String, dynamic>{
      'id': instance.id,
      'title': instance.title,
      'cover': instance.cover,
      'cover_small': instance.coverSmall,
      'cover_medium': instance.coverMedium,
      'cover_big': instance.coverBig,
      'cover_xl': instance.coverXl,
      'release_date': instance.releaseDate,
      'artist': instance.artist,
    };
