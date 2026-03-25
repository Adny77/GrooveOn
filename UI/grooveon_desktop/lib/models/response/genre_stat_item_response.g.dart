// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'genre_stat_item_response.dart';

// **************************************************************************
// JsonSerializableGenerator
// **************************************************************************

GenreStatItemResponse _$GenreStatItemResponseFromJson(
  Map<String, dynamic> json,
) => GenreStatItemResponse(
  genre: json['genre'] as String,
  playCount: (json['playCount'] as num).toInt(),
);

Map<String, dynamic> _$GenreStatItemResponseToJson(
  GenreStatItemResponse instance,
) => <String, dynamic>{
  'genre': instance.genre,
  'playCount': instance.playCount,
};
