// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'user_growth_point.dart';

// **************************************************************************
// JsonSerializableGenerator
// **************************************************************************

UserGrowthPoint _$UserGrowthPointFromJson(Map<String, dynamic> json) =>
    UserGrowthPoint(
      label: json['label'] as String,
      count: (json['count'] as num).toInt(),
    );

Map<String, dynamic> _$UserGrowthPointToJson(UserGrowthPoint instance) =>
    <String, dynamic>{'label': instance.label, 'count': instance.count};
